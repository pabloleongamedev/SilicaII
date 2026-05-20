/*
 * Arquitectura: Crafting/UI
 * Script: ToolChemistryView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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

        NotificationEvents.PublishNotification(new NotificationData
        {
            message = msg,
            type = type
        });
    }
}
