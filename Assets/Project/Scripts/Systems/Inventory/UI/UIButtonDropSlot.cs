/*
 * Arquitectura: Inventory/UI
 * Script: UIButtonDropSlot
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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