/*
 * Arquitectura: Inventory/UI
 * Script: InventoryListItemView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class InventoryListItemView : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IDropHandler, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;

    private InventoryDragHandler dragHandler;
    private InventoryItemInstance currentItem;
    private int index;

    public Action<int, int> OnItemDropped;
    public Action<InventoryItemInstance> OnItemClicked;

    public void Initialize(int index, InventoryDragHandler dragHandler)
    {
        this.index = index;
        this.dragHandler = dragHandler;
    }

    public void SetItem(InventoryItemInstance item)
    {
        currentItem = item;

        if (item == null)
        {
            icon.enabled = false;
            icon.sprite = null;
            nameText.text = "";
            amountText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = item.Data.icon;

        // 🔥 FIX: usar displayName (no itemID)
        nameText.text = item.Data.displayName;

        amountText.text = item.Quantity > 1 ? $"x{item.Quantity}" : "";
    }

    // CLICK
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;
        OnItemClicked?.Invoke(currentItem);
    }

    // DRAG
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null || dragHandler == null) return;
        dragHandler.StartDrag(currentItem.Data.icon);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragHandler == null) return;
        dragHandler.UpdateDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragHandler == null) return;
        dragHandler.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var from = eventData.pointerDrag?.GetComponent<InventoryListItemView>();
        if (from == null) return;

        OnItemDropped?.Invoke(from.index, this.index);
    }

    public InventoryItemInstance GetItem()
    {
        return currentItem;
    }

    public ItemData_SO GetItemData()
    {
        return currentItem != null ? currentItem.Data : null;
    }
}