/*
 * Arquitectura: Inventory/Runtime
 * Script: InventoryController
 * Rol: Facade runtime de Inventory. Construye el Core desde configuracion y expone contratos de lectura/escritura.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Interaction y Crafting consumen IInventoryReadModel/IInventoryWriteModel; UI escucha ReadModel; SaveLoad exporta/importa InventorySaveData.
 * Riesgo arquitectonico mitigado: ya no publica QuestEvents/GameplayEvents directamente; GameplayEventRouter traduce InventoryEvents hacia Quest/Notification.
 * Uso como referencia: buen ejemplo de Core desacoplado con facade runtime y eventos propios del sistema.
 */
using UnityEngine;
using System;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private InventoryConfig_SO config;

    [Header("View")]
    [SerializeField] private InventoryView inventoryView;
    [SerializeField] private InventoryListView listView;

    public IInventoryReadModel ReadModel => inventorySystem.ReadModel;
    public IInventoryWriteModel WriteModel => inventorySystem;

    private InventorySystem inventorySystem;
    private InventoryGrid grid;

    private void Awake()
    {
        // Facade Runtime: crea el Core desde datos editables y traduce eventos
        // de dominio a eventos consumidos por UI, Quest y Notification.
        grid = new InventoryGrid(config.width, config.height);
        inventorySystem = new InventorySystem(config, grid);
        inventorySystem.OnNotificationRequested += HandleNotificationRequested;
        inventorySystem.OnItemAdded += HandleItemAdded;
        inventorySystem.OnItemRemoved += HandleItemRemoved;

        if (inventoryView == null)
            Debug.LogError("InventoryView not assigned");
    }

    private void Start()
    {
        if (inventoryView != null)
        {
            inventoryView.Initialize(inventorySystem.ReadModel);
            inventoryView.OnItemDropped += MoveItem;
            inventoryView.ForceRefresh();
        }

        if (listView != null)
        {
            listView.Initialize(inventorySystem.ReadModel);
            listView.OnItemDropped += MoveItem;
        }
    }

    private void OnDestroy()
    {
        if (inventoryView != null)
            inventoryView.OnItemDropped -= MoveItem;

        if (listView != null)
            listView.OnItemDropped -= MoveItem;

        if (inventorySystem != null)
        {
            inventorySystem.OnNotificationRequested -= HandleNotificationRequested;
            inventorySystem.OnItemAdded -= HandleItemAdded;
            inventorySystem.OnItemRemoved -= HandleItemRemoved;
        }
    }

    public int TryAddItem(ItemData_SO data, int amount)
    {
        return inventorySystem.AddItem(data, amount);
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        inventorySystem.MoveItem(fromIndex, toIndex);
    }

    public void ResetInventory()
    {
        inventorySystem.Clear();

        if (inventoryView != null)
            inventoryView.ForceRefresh();
    }

    public List<InventorySaveData> ExportSaveData()
    {
        return inventorySystem.ExportSaveData();
    }

    public void ImportSaveData(IEnumerable<InventorySaveData> savedItems, Func<string, ItemData_SO> resolveItem)
    {
        inventorySystem.ImportSaveData(savedItems, resolveItem);

        if (inventoryView != null)
            inventoryView.ForceRefresh();
    }

    public InventorySystem GetInventorySystem()
    {
        return inventorySystem;
    }

    private void HandleNotificationRequested(InventoryFeedback feedback)
    {
        // Adapter entre Core y UI: InventorySystem no conoce NotificationData.
        var notification = new NotificationData
        {
            message = feedback.message,
            type = ToNotificationType(feedback.type)
        };

        InventoryEvents.OnNotificationRequested?.Invoke(notification);
    }

    private void HandleItemAdded(ItemData_SO item, int amount)
    {
        // Evento propio de Inventory: GameplayEventRouter decide si esto avanza Quest.
        InventoryEvents.OnItemAdded?.Invoke(item, amount);
    }

    private void HandleItemRemoved(ItemData_SO item, int amount)
    {
        InventoryEvents.OnItemRemoved?.Invoke(item, amount);
    }

    private NotificationType ToNotificationType(InventoryFeedbackType type)
    {
        switch (type)
        {
            case InventoryFeedbackType.Success: return NotificationType.Success;
            case InventoryFeedbackType.Error: return NotificationType.Error;
            case InventoryFeedbackType.Info: return NotificationType.Info;
            default: return NotificationType.Warning;
        }
    }
}
