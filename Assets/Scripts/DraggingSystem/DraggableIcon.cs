using Scriptables;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableIcon : MonoBehaviour, IPointerDownHandler
{
    public ItemData data;
    [HideInInspector] public IDropTarget slot;   // whichever slot (lobby, puzzle, ...) currently holds this icon
    [HideInInspector] public bool locked;        // optional: set true by a slot to make the icon unpickable

    public void SetTagMode(bool on)
    {
        var label = GetComponentInChildren<TMP_Text>(true);
        if (label)
        {
            label.gameObject.SetActive(on);
            label.text = data.displayName;
        }
    }

    /// <summary>Parent under a slot and stretch to fill it. Shared by every slot type.</summary>
    public void SnapInto(Transform slotTransform)
    {
        var rt = (RectTransform)transform;
        rt.SetParent(slotTransform, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (locked) return;
        slot?.Release(this);                // free the slot when picked up
        DragManager.Instance.BeginDrag(data);
        Destroy(gameObject);
    }
}
