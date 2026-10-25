using System.Collections;
using System.Collections.Generic;
using Scriptables;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance;

    /// <summary>Slots register here from OnEnable / OnDisable. Replaces the old slotPanel lookup.</summary>
    public static readonly List<IDropTarget> Targets = new List<IDropTarget>();

    [Header("Room")]
    public RoomWindow room;

    [Header("UI")]
    public RectTransform canvasRoot;
    public Image ghostPrefab;

    [Header("Feel")]
    [Tooltip("Used when an ItemData has no DragFeel. Leave empty for the old instant-follow behaviour.")]
    public DragFeel defaultFeel;

    public float hover = 0.25f;

    ItemData heldPlayer;
    RoomItem source3D;
    Image ghost2DPlayer;
    GameObject proxy3D;
    Vector3 startPos;
    Vector3 _lastFloorPoint;

    DragFeel feel;
    DragMotion ghostMotion, motion3D;
    Vector3 ghostBaseScale;
    float tilt;
    bool proxyShown;
    IDropTarget hoverTarget;

    void Awake() => Instance = this;

    public void BeginDrag(ItemData data, RoomItem from = null)
    {
        heldPlayer = data;
        source3D = from;
        feel = data.feel != null ? data.feel : defaultFeel;
        if (from) startPos = from.transform.position;

        Vector2 mouse = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;

        ghost2DPlayer = Instantiate(ghostPrefab, canvasRoot);
        ghost2DPlayer.sprite = data.icon;
        ghost2DPlayer.color = data.colour;
        ghost2DPlayer.transform.position = mouse;
        ghostBaseScale = ghost2DPlayer.transform.localScale;
        tilt = 0f;

        // New motion objects per drag: a settle coroutine from the previous drag may still own the old ones.
        ghostMotion = new DragMotion();
        ghostMotion.Reset(mouse);
        motion3D = new DragMotion();

        if (from)
        {
            proxy3D = from.gameObject;
            motion3D.Reset(from.transform.position);
            proxyShown = true;
        }
        else
        {
            proxy3D = Instantiate(data.prefab3D);
            // proxy3D.transform.rotation = new Quaternion(0, -90, 0, 0);

            var roomItem = proxy3D.GetComponent<RoomItem>();
            if (roomItem == null) roomItem = proxy3D.AddComponent<RoomItem>();
            roomItem.data = data;
            roomItem.ApplyColour();

            SetLayerRecursive(proxy3D, LayerMask.NameToLayer("3D-roomItem"));
            proxy3D.SetActive(false);
            proxyShown = false;
        }
    }

    void Update()
    {
        if (heldPlayer == null || Mouse.current == null) return;

        float dt = Time.unscaledDeltaTime;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        bool mouseIn3D = room.Contains(mousePos);

        //2D-icon keeps following mouse but disabled to allow for seamless 
        Follow(ghostMotion, mousePos, dt, false);
        ghost2DPlayer.transform.position = ghostMotion.Position;
        ApplyGhostLook(dt);
        ghost2DPlayer.gameObject.SetActive(!mouseIn3D);

        //hover follows mouse not prefab since it's designed to lag behind
        SetHover(mouseIn3D ? null : FindTarget(mousePos));

        //3D proxy
        Vector3 pt = default;
        bool onFloor = mouseIn3D && room.TryGetFloorPoint(mousePos, out pt);
        proxy3D.SetActive(onFloor);
        if (onFloor)
        {
            _lastFloorPoint = pt;
            Vector3 target = pt + Vector3.up * (BaseOffset(proxy3D) + hover);

            if (!proxyShown) motion3D.Reset(target);   // appear under the cursor, not flying in from the origin
            else Follow(motion3D, target, dt, true);

            proxy3D.transform.position = motion3D.Position;
        }
        proxyShown = onFloor;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            EndDrag(mousePos, mouseIn3D, onFloor);
    }

    void EndDrag(Vector2 mouseP, bool inside, bool onFloor)
    {
        SetHover(null);

        //Loose/slotted icons should start from their normal scale and rotation
        ghost2DPlayer.transform.localScale = ghostBaseScale;
        ghost2DPlayer.transform.localRotation = Quaternion.identity;

        if (onFloor)
        {
            Vector3 final = _lastFloorPoint + Vector3.up * BaseOffset(proxy3D);
            StartCoroutine(Settle(proxy3D.transform, motion3D, final, feel));
            Destroy(ghost2DPlayer.gameObject);
        }
        else if (inside)
        {
            //Released over the window but not on the floor == cancel
            if (source3D != null)
            {
                proxy3D.SetActive(true);
                StartCoroutine(Settle(proxy3D.transform, motion3D, startPos, feel));
            }
            else Destroy(proxy3D);

            Destroy(ghost2DPlayer.gameObject);
        }
        else
        {
            //player is dropped outside the window == becomes a 2D icon, handed to a drop target if one is under the cursor
            Destroy(proxy3D);
            ghost2DPlayer.gameObject.SetActive(true);
            ghost2DPlayer.raycastTarget = true;

            var icon = ghost2DPlayer.gameObject.AddComponent<DraggableIcon>();
            icon.data = heldPlayer;

            var target = FindTarget(mouseP);
            if (target != null)
            {
                if (target.CanAccept(heldPlayer)) target.Drop(icon);
                else target.Reject(icon);
            }
        }

        heldPlayer = null;
        source3D = null;
        proxy3D = null;
        ghost2DPlayer = null;
        feel = null;
    }

    // ---------- feel helpers ----------

    void Follow(DragMotion m, Vector3 target, float dt, bool world)
    {
        if (feel != null) m.Step(target, feel, dt, world);
        else m.Reset(target);                       // no feel assigned: instant follow like before
    }

    void ApplyGhostLook(float dt)
    {
        if (feel == null) return;

        var tf = ghost2DPlayer.transform;
        Vector3 wantScale = ghostBaseScale * feel.pickupScale;
        tf.localScale = Vector3.Lerp(tf.localScale, wantScale, 1f - Mathf.Exp(-feel.scaleSpeed * dt));

        float wantTilt = Mathf.Clamp(-ghostMotion.Velocity.x * feel.tiltPerSpeed, -feel.maxTilt, feel.maxTilt);
        tilt = Mathf.Lerp(tilt, wantTilt, 1f - Mathf.Exp(-feel.tiltSmoothing * dt));
        tf.localRotation = Quaternion.Euler(0f, 0f, tilt);
    }

    /// <summary>Lets the 3D proxy glide to its final spot after release instead of popping there.</summary>
    IEnumerator Settle(Transform t, DragMotion m, Vector3 finalPos, DragFeel f)
    {
        if (f != null)
        {
            float timeout = 1.5f;
            while (t != null && timeout > 0f &&
                   ((m.Position - finalPos).sqrMagnitude > 1e-4f || m.Velocity.sqrMagnitude > 1e-3f))
            {
                // Picked up again mid-settle: let the new drag take over
                if (source3D != null && source3D.transform == t) yield break;

                float dt = Time.unscaledDeltaTime;
                m.Step(finalPos, f, dt, true);
                t.position = m.Position;
                timeout -= dt;
                yield return null;
            }
        }
        if (t != null && !(source3D != null && source3D.transform == t))
            t.position = finalPos;
    }

    // ---------- drop targets ----------

    IDropTarget FindTarget(Vector2 p)
    {
        foreach (var t in Targets)
            if (t.Contains(p)) return t;
        return null;
    }

    void SetHover(IDropTarget t)
    {
        if (t == hoverTarget) return;
        hoverTarget?.HoverExit();
        hoverTarget = t;
        hoverTarget?.HoverEnter(heldPlayer);
    }

    void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform t in go.transform) SetLayerRecursive(t.gameObject, layer);
    }

    float BaseOffset(GameObject go)
    { // offset of the mesh from its pivot
        var r = go.GetComponentInChildren<Renderer>();
        return r ? go.transform.position.y - r.bounds.min.y : 0f;
    }
}
