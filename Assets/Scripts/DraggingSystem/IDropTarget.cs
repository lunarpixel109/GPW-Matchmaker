using Scriptables;
using UnityEngine;

/// <summary>
/// Anything the DragManager can drop a 2D icon onto. Implementers register themselves in
/// DragManager.Targets from OnEnable and remove themselves in OnDisable.
/// </summary>
public interface IDropTarget
{
    bool Contains(Vector2 screenPos);
    bool CanAccept(ItemData item);

    void HoverEnter(ItemData item);   // pointer moved over this target while carrying item
    void HoverExit();

    void Drop(DraggableIcon icon);    // only called if CanAccept returned true
    void Reject(DraggableIcon icon);  // released over this target but CanAccept was false
    void Release(DraggableIcon icon); // an icon sitting in this target was picked back up
}
