/*
 * Arquitectura: Crafting/UI
 * Script: CraftingSlotView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingSlotView : MonoBehaviour,
    IDropHandler,
    IBeginDragHandler,
    IEndDragHandler
{
    [SerializeField] private Image icon;

    private ItemData_SO currentItem;

    public Action<CraftingSlotView, ItemData_SO> OnItemDropped;
    public Action<CraftingSlotView, ItemData_SO> OnItemBeginDrag;

    public ItemData_SO GetItem() => currentItem;

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<InventoryListItemView>();
        if (dragged == null) return;

        var item = dragged.GetItemData();
        if (item == null) return;

        OnItemDropped?.Invoke(this, item);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        OnItemBeginDrag?.Invoke(this, currentItem);
    }

    public void OnEndDrag(PointerEventData eventData) { }

    public void SetItem(ItemData_SO item)
    {
        currentItem = item;

        if (icon == null) return;

        icon.sprite = item.icon;
        icon.enabled = true;
        icon.color = Color.white;
        icon.rectTransform.localScale = Vector3.one;
        icon.transform.SetAsLastSibling();
    }

    public void Clear()
    {
        currentItem = null;

        if (icon == null) return;

        icon.sprite = null;
        icon.enabled = false;
    }
}
