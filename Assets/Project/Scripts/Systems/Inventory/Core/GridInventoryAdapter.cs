using System;

public class GridInventoryAdapter : IInventoryReadModel
{
    private InventoryGrid grid;
    private Func<(ItemData_SO item, int amount)[], bool> canAddItemsBatch;
    private Func<(ItemData_SO item, int amount)[], (ItemData_SO item, int amount)[], bool> canProcessBatch;

    public int Capacity => grid.Width * grid.Height;

    public event Action<int, InventoryItemInstance> OnItemChanged;

    public GridInventoryAdapter(
        InventoryGrid grid,
        Func<(ItemData_SO item, int amount)[], bool> canAddItemsBatch,
        Func<(ItemData_SO item, int amount)[], (ItemData_SO item, int amount)[], bool> canProcessBatch)
    {
        this.grid = grid;
        this.canAddItemsBatch = canAddItemsBatch;
        this.canProcessBatch = canProcessBatch;
    }

    // =========================================================
    // GET ITEM
    // =========================================================
    public InventoryItemInstance GetItem(int index)
    {
        if (index < 0 || index >= Capacity)
            return null;

        int x = index % grid.Width;
        int y = index / grid.Width;

        var slot = grid.GetSlot(x, y);
        return slot?.ItemInstance;
    }

    // =========================================================
    // GET AMOUNT
    // =========================================================
    public int GetAmount(ItemData_SO item)
    {
        int total = 0;

        foreach (var slot in grid.GetAllSlots())
        {
            if (slot.IsEmpty)
                continue;

            if (slot.ItemInstance.Data == item)
                total += slot.ItemInstance.Quantity;
        }

        return total;
    }

    // =========================================================
    // NOTIFY SLOT
    // =========================================================
    public void NotifySlotChanged(int x, int y, InventoryItemInstance item)
    {
        int index = y * grid.Width + x;
        OnItemChanged?.Invoke(index, item);
    }

    // =========================================================
    // VALIDATION (SIMPLE)
    // =========================================================
    public bool CanAddItem(ItemData_SO item, int amount)
    {
        return canAddItemsBatch?.Invoke(new[] { (item, amount) }) ?? false;
    }

    // =========================================================
    // VALIDATION (BATCH)
    // =========================================================
    public bool CanAddItemsBatch(params (ItemData_SO item, int amount)[] items)
    {
        return canAddItemsBatch?.Invoke(items) ?? false;
    }

    // =========================================================
    // PROCESS VALIDATION
    // =========================================================
    public bool CanProcessBatch(
        (ItemData_SO item, int amount)[] remove,
        (ItemData_SO item, int amount)[] add)
    {
        return canProcessBatch?.Invoke(remove, add) ?? false;
    }
}
