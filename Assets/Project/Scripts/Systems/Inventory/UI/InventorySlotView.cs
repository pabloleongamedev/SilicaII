/*
 * Arquitectura: Inventory/UI
 * Script: InventorySlotView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class InventorySlotView : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IDropHandler, IPointerClickHandler
{
    private int index;

    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI stackText;

    private InventoryDragHandler dragHandler;
    private InventoryItemInstance currentItem;
    private int currentAmount;

    public Action<InventoryItemInstance> OnSlotClicked;
    public Action<int, int> OnItemDropped;

    public void Initialize(int index, InventoryDragHandler dragHandler)
    {
        this.index = index;
        this.dragHandler = dragHandler;

        
    }
    public void SetItem(InventoryItemInstance item, int amount)
    {
        currentItem = item;
        currentAmount = amount;

        if (item == null || amount <= 0)
        {
            currentItem = null;
            currentAmount = 0;

            icon.enabled = false;
            icon.sprite = null;
            stackText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = item.Data.icon;
        stackText.text = amount > 1 ? amount.ToString() : "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICK SLOT"); // 👈

        OnSlotClicked?.Invoke(currentItem);
    }

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
        var fromSlot = eventData.pointerDrag?.GetComponent<InventorySlotView>();
        if (fromSlot == null) return;

        OnItemDropped?.Invoke(fromSlot.index, this.index);
    }
}