using System.Collections;
using Scriptables;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Accepts one specific ItemData, shows green/red hover feedback, and holds its icon like a LobbySlot does,
/// so a placed icon can be picked back up and moved to another slot, the room, or loose on the canvas.
/// </summary>
[RequireComponent(typeof(Image))]
public class PuzzleSlot : MonoBehaviour, IDropTarget
{
    public ItemData accepts;
    public PuzzleManager manager;
    public DraggableIcon occupant;

    public Color neutral = new Color(1f, 1f, 1f, 0.4f);
    public Color correct = new Color(0.35f, 0.8f, 0.4f, 0.8f);
    public Color wrong   = new Color(0.9f, 0.3f, 0.3f, 0.8f);

    Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        img.color = neutral;
    }

    void OnEnable()  => DragManager.Targets.Add(this);
    void OnDisable() => DragManager.Targets.Remove(this);

    public bool Contains(Vector2 screenPos) =>
        RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, screenPos, null);

    public bool CanAccept(ItemData item) => occupant == null && item == accepts;

    public void HoverEnter(ItemData item)
    {
        if (occupant != null) return;
        img.color = item == accepts ? correct : wrong;
    }

    public void HoverExit()
    {
        if (occupant == null) img.color = neutral;
    }

    public void Drop(DraggableIcon icon)
    {
        occupant = icon;
        icon.slot = this;
        img.color = correct;
        icon.SnapInto(transform);
        icon.SetTagMode(true);
        if (manager) manager.SlotFilled();
        else Debug.LogWarning("PuzzleSlot has no PuzzleManager assigned.", this);
    }

    public void Release(DraggableIcon icon)
    {
        if (occupant != icon) return;
        occupant = null;
        img.color = neutral;
        if (manager) manager.SlotEmptied();
    }

    public void Reject(DraggableIcon icon)
    {
        if (occupant != null) return;
        img.color = wrong;
        if (manager) manager.WrongSlot();
        StopAllCoroutines();
        StartCoroutine(FlashReset(0.4f));
    }

    IEnumerator FlashReset(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (occupant == null) img.color = neutral;
    }
}
