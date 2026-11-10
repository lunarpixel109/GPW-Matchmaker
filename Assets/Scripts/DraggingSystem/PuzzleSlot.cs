using System.Collections;
using Scriptables;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Accepts one specific ItemData (or any item if requireCorrectItem is off), shows green/red hover feedback,
/// and holds its icon like a LobbySlot does, so a placed icon can be picked back up and moved elsewhere.
/// </summary>
[RequireComponent(typeof(Image))]
public class PuzzleSlot : MonoBehaviour, IDropTarget
{
    [Tooltip("If on, only 'accepts' can be dropped here. If off, any item is allowed.")]
    public bool requireCorrectItem = true;

    public ItemData accepts;
    public PuzzleManager manager;
    public DraggableIcon occupant;

    public Color neutral = new Color(1f, 1f, 1f, 0.4f);
    public Color correct = new Color(0.35f, 0.8f, 0.4f, 0.8f);
    public Color wrong   = new Color(0.9f, 0.3f, 0.3f, 0.8f);

    Image img;

    /// <summary>True if the slot is filled with the item it expects (always true for any filled slot when checking is off).</summary>
    public bool IsCorrectlyFilled =>
        occupant != null && (!requireCorrectItem || occupant.data == accepts);

    void Awake()
    {
        img = GetComponent<Image>();
        img.color = neutral;
    }

    void OnEnable()  => DragManager.Targets.Add(this);
    void OnDisable() => DragManager.Targets.Remove(this);

    public bool Contains(Vector2 screenPos) =>
        RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, screenPos, null);

    bool Matches(ItemData item) => !requireCorrectItem || item == accepts;

    public bool CanAccept(ItemData item) => occupant == null && Matches(item);

    public void HoverEnter(ItemData item)
    {
        if (occupant != null) return;
        img.color = Matches(item) ? correct : wrong;
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