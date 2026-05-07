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

    private void HandleNotificationRequested(NotificationData notification)
    {
        InventoryEvents.OnNotificationRequested?.Invoke(notification);
        GameplayEvents.OnNotification?.Invoke(notification);
    }

    private void HandleItemAdded(ItemData_SO item, int amount)
    {
        InventoryEvents.OnItemAdded?.Invoke(item, amount);
        QuestEvents.OnItemCollected?.Invoke(item, amount);
    }

    private void HandleItemRemoved(ItemData_SO item, int amount)
    {
        InventoryEvents.OnItemRemoved?.Invoke(item, amount);
    }
}
