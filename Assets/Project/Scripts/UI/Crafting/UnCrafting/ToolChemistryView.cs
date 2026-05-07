using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ToolChemistryView : MonoBehaviour, IDropHandler
{
    [SerializeField] private ChemistrySlotView slot;
    [SerializeField] private SeparationDatabase_SO database;

    private ItemData_SO currentItem;

    public Action<ItemData_SO> OnItemPlaced;
    public Action OnItemCleared;

    public void OnDrop(PointerEventData eventData)
    {
        var dragGO = eventData.pointerDrag;
        if (dragGO == null) return;

        var itemView = dragGO.GetComponent<InventoryListItemView>();
        if (itemView == null) return;

        var itemInstance = itemView.GetItem();
        if (itemInstance == null) return;

        var item = itemInstance.Data;

        // =====================================================
        // 🔥 VALIDACIÓN + NOTIFICACIÓN
        // =====================================================
        var compound = database.Get(item);

        if (compound == null)
        {
            Notify("Este elemento no se puede refinar", NotificationType.Warning);
            return;
        }

        if (currentItem != null)
        {
            Notify("El refinador ya está ocupado", NotificationType.Warning);
            return;
        }

        // =====================================================
        // ✔ SOLO SI PASA VALIDACIÓN
        // =====================================================
        SetItem(item);
    }

    public void SetItem(ItemData_SO item)
    {
        currentItem = item;
        slot.SetItem(item);

        OnItemPlaced?.Invoke(item);
    }

    public void Clear()
    {
        if (currentItem == null)
            return;

        currentItem = null;
        slot.Clear();

        OnItemCleared?.Invoke();
    }

    public ItemData_SO GetItem() => currentItem;

    // =====================================================
    // 🔥 SISTEMA DE NOTIFICACIÓN CENTRALIZADO
    // =====================================================
    private void Notify(string msg, NotificationType type)
    {
        Debug.Log("[Chemistry][Drop] " + msg);

        GameplayEvents.OnNotification?.Invoke(new NotificationData
        {
            message = msg,
            type = type
        });
    }
}