/*
 * Arquitectura: Inventory/UI
 * Script: InventoryListView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryListView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InventoryListItemView itemPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private InventoryDragHandler dragHandler;

    private IInventoryReadModel inventory;
    private List<InventoryListItemView> items = new List<InventoryListItemView>();

    public Action<int, int> OnItemDropped;
    public Action<InventoryItemInstance> OnItemClicked;

    public void Initialize(IInventoryReadModel inventory)
    {
        if (this.inventory != null)
            this.inventory.OnItemChanged -= UpdateSlot;

        this.inventory = inventory;

        Build();

        // 🔥 FIX CRÍTICO: evitar múltiples suscripciones
        inventory.OnItemChanged -= UpdateSlot;
        inventory.OnItemChanged += UpdateSlot;
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnItemChanged -= UpdateSlot;
    }

    private void Build()
    {
        // limpiar
        foreach (var item in items)
            Destroy(item.gameObject);

        items.Clear();

        // 🔥 IMPORTANTE: SIEMPRE usar Capacity (1:1 con grid)
        for (int i = 0; i < inventory.Capacity; i++)
        {
            var itemView = Instantiate(itemPrefab, container);

            itemView.Initialize(i, dragHandler);

            itemView.OnItemDropped += HandleDrop;
            itemView.OnItemClicked += HandleClick;

            items.Add(itemView);

            var item = inventory.GetItem(i);
            itemView.SetItem(item);
        }
    }

    private void UpdateSlot(int index, InventoryItemInstance item)
    {
        if (index < 0 || index >= items.Count)
            return;

        // 🔥 FIX CRÍTICO: SIEMPRE consultar el modelo real
        var realItem = inventory.GetItem(index);

        items[index].SetItem(realItem);
    }

    private void HandleDrop(int fromIndex, int toIndex)
    {
        Debug.Log($"[InventoryListView] DROP from {fromIndex} to {toIndex}");

        OnItemDropped?.Invoke(fromIndex, toIndex);
    }

    private void HandleClick(InventoryItemInstance item)
    {
        OnItemClicked?.Invoke(item);
    }
}
