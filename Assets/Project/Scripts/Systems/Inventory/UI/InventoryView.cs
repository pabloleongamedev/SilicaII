/*
 * Arquitectura: Inventory/UI
 * Script: InventoryView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InventorySlotView slotPrefab;
    [SerializeField] private Transform InventoryGridContainer;
    [SerializeField] private DescriptionPanelView descriptionPanel;
    [SerializeField] private InventoryDragHandler dragHandler;
    [SerializeField] private GameObject QuestPanel;
    [SerializeField] private GameObject inventoryPanel; 

    private IInventoryReadModel inventory;
    private List<InventorySlotView> slotViews = new List<InventorySlotView>();

    public Action<int, int> OnItemDropped;

    public void Initialize(IInventoryReadModel inventory)
    {
        if (this.inventory != null)
            this.inventory.OnItemChanged -= UpdateSlot;

        this.inventory = inventory;

        Build();
        inventory.OnItemChanged += UpdateSlot;
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnItemChanged -= UpdateSlot;
    }

    private void Build()
    {
        foreach (var slotView in slotViews)
            Destroy(slotView.gameObject);

        slotViews.Clear();
        
        for (int i = 0; i < inventory.Capacity; i++)
        {
            var slot = Instantiate(slotPrefab, InventoryGridContainer);

            // 🔥 INYECCIÓN
            slot.Initialize(i, dragHandler);

            slot.OnSlotClicked += HandleSlotClicked;
            slot.OnItemDropped += HandleItemDropped;

            slotViews.Add(slot);

            var item = inventory.GetItem(i);
            slot.SetItem(item, item != null ? item.Quantity : 0);
        }
    }

    private void UpdateSlot(int index, InventoryItemInstance item)
    {
        if (index < 0 || index >= slotViews.Count)
            return;

        // 🔥 SIEMPRE leer del modelo real
        var realItem = inventory.GetItem(index);

        slotViews[index].SetItem(realItem, realItem != null ? realItem.Quantity : 0);
    }

    private void HandleSlotClicked(InventoryItemInstance item)
    {
        if (descriptionPanel == null)
        {
            Debug.LogError("DescriptionPanel not assigned");
            return;
        }

        descriptionPanel.Show(item);
    }

    private void HandleItemDropped(int fromIndex, int toIndex)
    {
        OnItemDropped?.Invoke(fromIndex, toIndex);
    }

    // =========================================
    // UI TOGGLE
    // =========================================

    public void ShowQuestPanel()
    {
        if (QuestPanel != null)
            QuestPanel.SetActive(true);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        UIStateEvents.OnUIStateChanged?.Invoke(UIState.Quest);
    }

    public void ShowInventoryPanel()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        if (QuestPanel != null)
            QuestPanel.SetActive(false);
    }

    public void ForceRefresh()
    {
        for (int i = 0; i < slotViews.Count; i++)
        {
            var item = inventory.GetItem(i);
            slotViews[i].SetItem(item, item != null ? item.Quantity : 0);
        }
    }
}
