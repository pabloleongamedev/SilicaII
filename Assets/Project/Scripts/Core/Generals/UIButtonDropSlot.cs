using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIButtonDropSlot : MonoBehaviour, IDropHandler
{
    public Action<InventoryItemInstance> OnItemDropped;

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<InventoryListItemView>();

        if (dragged == null)
            return;

        var item = dragged.GetItem();

        if (item == null)
            return;

        Debug.Log("Item dropped on button: " + item.Data.displayName);

        OnItemDropped?.Invoke(item);
    }
}