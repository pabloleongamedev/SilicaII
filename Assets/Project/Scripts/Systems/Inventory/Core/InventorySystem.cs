/*
 * Arquitectura: Inventory/Core
 * Script: InventorySystem
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using System;
using System.Collections.Generic;

public class InventorySystem : IInventoryWriteModel
{
    private InventoryGrid grid;
    private InventoryOperations operations;
    private GridInventoryAdapter adapter;

    public IInventoryReadModel ReadModel { get; private set; }
    public event Action<ItemData_SO, int> OnItemAdded;
    public event Action<ItemData_SO, int> OnItemRemoved;
    public event Action<InventoryFeedback> OnNotificationRequested;

    public InventorySystem(InventoryConfig_SO config, InventoryGrid grid)
    {
        this.grid = grid;

        operations = new InventoryOperations(grid);
        adapter = new GridInventoryAdapter(grid, CanAddItemsBatch, CanProcessBatch);

        ReadModel = adapter;
    }

    // =========================================================
    // NOTIFICATION MODE (🔥 CONTROL PRO)
    // =========================================================
    public enum InventoryNotificationMode
    {
        None,   // sin notificación
        Silent, // lógica interna (sin UI)
        Normal  // notificación completa
    }

    // =========================================================
    // CALCULATION
    // =========================================================
    private int CalculateAddableAmount(ItemData_SO item, int amount)
    {
        int canAdd = 0;

        if (item.maxStack > 1)
        {
            foreach (var slot in grid.GetAllSlots())
            {
                if (slot.IsEmpty) continue;

                if (slot.ItemInstance.Data == item && !slot.ItemInstance.IsFull())
                {
                    canAdd += slot.ItemInstance.GetRemainingSpace();
                }
            }
        }

        if (canAdd < amount)
        {
            foreach (var slot in grid.GetAllSlots())
            {
                if (slot.IsEmpty)
                    canAdd += item.maxStack;

                if (canAdd >= amount)
                    break;
            }
        }

        return canAdd;
    }

    // =========================================================
    // ADD (INTERFAZ)
    // =========================================================
    public int AddItem(ItemData_SO item, int amount)
    {
        return AddItem(item, amount, InventoryNotificationMode.Normal);
    }

    // =========================================================
    // ADD SOBRECARGADO CON MODO DE NOTIFICACIÓN
    // =========================================================
    public int AddItem(ItemData_SO item, int amount, InventoryNotificationMode mode)
    {
        if (item == null || amount <= 0)
            return 0;

        int canAdd = CalculateAddableAmount(item, amount);

        if (canAdd <= 0)
        {
            if (mode == InventoryNotificationMode.Normal)
                Notify($"Inventario lleno para {item.itemID}", InventoryFeedbackType.Warning);
            return 0;
        }

        if (canAdd < amount)
        {
            if (mode == InventoryNotificationMode.Normal)
                Notify($"No hay espacio suficiente para {item.itemID}", InventoryFeedbackType.Warning);
            return 0;
        }

        int added = operations.AddItem(item, amount, NotifySlotChanged);

        if (added > 0 && mode == InventoryNotificationMode.Normal)
        {
            Notify($"Has obtenido {item.itemID} x{added}", InventoryFeedbackType.Success);
            OnItemAdded?.Invoke(item, added);
        }

        //  NOTIFICACIÓN A QUEST SYSTEM (CORRECTO)
        return added;
    }

    // =========================================================
    // REMOVE
    // =========================================================
    public void RemoveItem(ItemData_SO item, int amount)
    {
        RemoveItem(item, amount, InventoryNotificationMode.Normal);
    }

    public void RemoveItem(ItemData_SO item, int amount, InventoryNotificationMode mode)
    {
        int remaining = amount;
        int removedTotal = 0;

        if (item == null || amount <= 0)
            return;

        foreach (var slot in grid.GetAllSlots())
        {
            if (remaining <= 0) break;

            if (slot.IsEmpty || slot.ItemInstance.Data != item)
                continue;

            int removed = slot.ItemInstance.Remove(remaining);
            remaining -= removed;
            removedTotal += removed;

            if (slot.ItemInstance.IsEmpty())
            {
                slot.Clear();
                NotifySlotChanged(slot.X, slot.Y, null);
            }
            else
            {
                NotifySlotChanged(slot.X, slot.Y, slot.ItemInstance);
            }
        }

        if (removedTotal > 0)
        {
            if (mode == InventoryNotificationMode.Normal)
                Notify($"{item.itemID} x{removedTotal} consumido", InventoryFeedbackType.Info);

            OnItemRemoved?.Invoke(item, removedTotal);
        }
    }

    // =========================================================
    // MOVE
    // =========================================================
    public void MoveItem(int fromIndex, int toIndex)
    {
        int capacity = grid.Width * grid.Height;
        if (fromIndex < 0 || fromIndex >= capacity || toIndex < 0 || toIndex >= capacity)
            return;

        var from = IndexToGrid(fromIndex);
        var to = IndexToGrid(toIndex);

        var fromSlot = grid.GetSlot(from.x, from.y);
        var toSlot = grid.GetSlot(to.x, to.y);

        if (fromSlot.IsEmpty) return;

        if (!toSlot.IsEmpty && fromSlot.ItemInstance.Data == toSlot.ItemInstance.Data)
        {
            operations.Merge(from.x, from.y, to.x, to.y, NotifySlotChanged);
        }
        else
        {
            operations.Move(from.x, from.y, to.x, to.y, NotifySlotChanged);
        }
    }

    public void MoveItem(int fromX, int fromY, int toX, int toY)
    {
        if (!IsInBounds(fromX, fromY) || !IsInBounds(toX, toY))
            return;

        var fromSlot = grid.GetSlot(fromX, fromY);
        var toSlot = grid.GetSlot(toX, toY);

        if (fromSlot.IsEmpty) return;

        if (!toSlot.IsEmpty && fromSlot.ItemInstance.Data == toSlot.ItemInstance.Data)
            operations.Merge(fromX, fromY, toX, toY, NotifySlotChanged);
        else
            operations.Move(fromX, fromY, toX, toY, NotifySlotChanged);
    }

    public void MergeItem(int fromX, int fromY, int toX, int toY)
    {
        operations.Merge(fromX, fromY, toX, toY, NotifySlotChanged);
    }

    // =========================================================
    // CLEAR
    // =========================================================
    public void Clear()
    {
        Clear(InventoryNotificationMode.Normal);
    }

    public void Clear(InventoryNotificationMode mode)
    {
        foreach (var slot in grid.GetAllSlots())
        {
            if (!slot.IsEmpty)
            {
                slot.Clear();
                NotifySlotChanged(slot.X, slot.Y, null);
            }
        }

        if (mode == InventoryNotificationMode.Normal)
            Notify("Inventario limpiado", InventoryFeedbackType.Info);
    }

    // =========================================================
    // ACCESS
    // =========================================================
    public InventorySlot GetSlot(int x, int y) => grid.GetSlot(x, y);

    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < grid.Width && y >= 0 && y < grid.Height;
    }

    public (int x, int y) IndexToGrid(int index)
        => (index % grid.Width, index / grid.Width);

    public int GetAmount(ItemData_SO item)
    {
        int total = 0;

        foreach (var slot in grid.GetAllSlots())
        {
            if (!slot.IsEmpty && slot.ItemInstance.Data == item)
                total += slot.ItemInstance.Quantity;
        }

        return total;
    }

    // =========================================================
    // NOTIFY
    // =========================================================
    private void Notify(string message, InventoryFeedbackType type)
    {

        OnNotificationRequested?.Invoke(new InventoryFeedback
        {
            message = message,
            type = type
        });
    }

    private void NotifySlotChanged(int x, int y, InventoryItemInstance item)
    {
        adapter.NotifySlotChanged(x, y, item);
    }

    // =========================================================
    // CLONE (SIMULATION)
    // =========================================================
    private InventoryGrid CloneGrid()
    {
        var newGrid = new InventoryGrid(grid.Width, grid.Height);

        foreach (var slot in grid.GetAllSlots())
        {
            if (slot.IsEmpty) continue;

            var newInstance = new InventoryItemInstance(slot.ItemInstance.Data);
            newInstance.Add(slot.ItemInstance.Quantity);

            newGrid.GetSlot(slot.X, slot.Y).SetItem(newInstance);
        }

        return newGrid;
    }
        // =========================================================
    // VALIDATION (BATCH ADD)
    // =========================================================
    public bool CanAddItemsBatch(params (ItemData_SO item, int amount)[] items)
    {
        if (items == null)
            return true;

        var tempGrid = CloneGrid();
        var tempOps = new InventoryOperations(tempGrid);

        foreach (var (item, amount) in items)
        {
            if (item == null || amount <= 0)
                continue;

            int added = tempOps.AddItem(item, amount, null);

            if (added < amount)
                return false;
        }

        return true;
    }

    // =========================================================
    // VALIDATION (PROCESS)
    // =========================================================
    public bool CanProcessBatch(
        (ItemData_SO item, int amount)[] remove,
        (ItemData_SO item, int amount)[] add)
    {
        if (remove == null)
            remove = new (ItemData_SO item, int amount)[0];

        if (add == null)
            add = new (ItemData_SO item, int amount)[0];

        var tempGrid = CloneGrid();
        var tempOps = new InventoryOperations(tempGrid);

        // 🔥 REMOVE SIMULATION
        foreach (var (item, amount) in remove)
        {
            if (item == null || amount <= 0)
                continue;

            int remaining = amount;

            foreach (var slot in tempGrid.GetAllSlots())
            {
                if (remaining <= 0) break;

                if (slot.IsEmpty || slot.ItemInstance.Data != item)
                    continue;

                int removed = slot.ItemInstance.Remove(remaining);
                remaining -= removed;

                if (slot.ItemInstance.IsEmpty())
                    slot.Clear();
            }

            if (remaining > 0)
                return false;
        }

        // 🔥 ADD SIMULATION
        foreach (var (item, amount) in add)
        {
            if (item == null || amount <= 0)
                continue;

            int added = tempOps.AddItem(item, amount, null);

            if (added < amount)
                return false;
        }

        return true;
    }

    public bool TryProcessBatch(
        (ItemData_SO item, int amount)[] remove,
        (ItemData_SO item, int amount)[] add)
    {
        if (!CanProcessBatch(remove, add))
            return false;

        foreach (var (item, amount) in remove)
            RemoveItem(item, amount, InventoryNotificationMode.Silent);

        foreach (var (item, amount) in add)
            AddItem(item, amount, InventoryNotificationMode.Silent);

        return true;
    }

    public List<InventorySaveData> ExportSaveData()
    {
        var data = new List<InventorySaveData>();

        foreach (var slot in grid.GetAllSlots())
        {
            if (slot.IsEmpty)
                continue;

            data.Add(new InventorySaveData
            {
                itemID = slot.ItemInstance.Data.itemID,
                gridX = slot.X,
                gridY = slot.Y,
                quantity = slot.ItemInstance.Quantity
            });
        }

        return data;
    }

    public void ImportSaveData(IEnumerable<InventorySaveData> savedItems, Func<string, ItemData_SO> resolveItem)
    {
        Clear(InventoryNotificationMode.Silent);

        if (savedItems == null || resolveItem == null)
            return;

        foreach (var savedItem in savedItems)
        {
            var itemData = resolveItem(savedItem.itemID);

            if (itemData == null || savedItem.quantity <= 0)
                continue;

            if (savedItem.gridX < 0 || savedItem.gridX >= grid.Width ||
                savedItem.gridY < 0 || savedItem.gridY >= grid.Height)
                continue;

            var slot = grid.GetSlot(savedItem.gridX, savedItem.gridY);
            if (!slot.IsEmpty)
                continue;

            var instance = new InventoryItemInstance(itemData);
            instance.Add(savedItem.quantity);
            slot.SetItem(instance);
            NotifySlotChanged(savedItem.gridX, savedItem.gridY, instance);
        }
    }
}
