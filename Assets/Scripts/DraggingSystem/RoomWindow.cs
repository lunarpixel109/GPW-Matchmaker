using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomWindow : MonoBehaviour, IPointerDownHandler
{
    public RawImage view;
    public Camera roomCamera;
    public LayerMask pickMask, floorMask;

    bool TryGetRay(Vector2 screenPos, out Ray ray)
    {
        ray = default;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                view.rectTransform, screenPos, null, out var local)) return false; // null = Overlay canvas
        var r = view.rectTransform.rect;
        var uv = new Vector2((local.x - r.x) / r.width, (local.y - r.y) / r.height);
        //uv = view.uvRect.position + Vector2.Scale(uv, view.uvRect.size);   // crop remap
        ray = roomCamera.ViewportPointToRay(uv);
        return true;
    }

    public bool Contains(Vector2 screenPos) =>
        RectTransformUtility.RectangleContainsScreenPoint(view.rectTransform, screenPos, null);

    public bool TryGetFloorPoint(Vector2 screenPos, out Vector3 point)
    {
        point = default;
        if (TryGetRay(screenPos, out var ray) && Physics.Raycast(ray, out var hit, 100f, floorMask))
        { point = hit.point; return true; }
        return false;
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (TryGetRay(e.position, out var ray) &&
            Physics.Raycast(ray, out var hit, 100f, pickMask))
        {
            var item = hit.collider.GetComponentInParent<RoomItem>();
            if (item) DragManager.Instance.BeginDrag(item.data, item);
        }
    }
}