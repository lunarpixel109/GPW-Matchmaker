using Scriptables;
using UnityEngine;

public class LobbySlot : MonoBehaviour, IDropTarget
{
    public DraggableIcon occupant;

    void OnEnable()  => DragManager.Targets.Add(this);
    void OnDisable() => DragManager.Targets.Remove(this);

    public bool Contains(Vector2 screenPos) =>
        RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, screenPos, null);

    public bool CanAccept(ItemData item) => occupant == null;

    public void HoverEnter(ItemData item) { }
    public void HoverExit() { }
    public void Reject(DraggableIcon icon) { }   // icon just stays loose where it was released

    public void Release(DraggableIcon icon)
    {
        if (occupant == icon) occupant = null;
    }

    public void Drop(DraggableIcon icon) => Place(icon);

    public void Place(DraggableIcon icon)
    {
        occupant = icon;
        icon.slot = this;
        icon.SnapInto(transform);
        icon.SetTagMode(true);
    }
}
