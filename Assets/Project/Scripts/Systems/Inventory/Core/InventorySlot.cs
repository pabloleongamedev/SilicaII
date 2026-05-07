public class InventorySlot
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public InventoryItemInstance ItemInstance;

    public bool IsEmpty => ItemInstance == null;

    public ItemData_SO ItemData => ItemInstance?.Data;
    public int Quantity => ItemInstance?.Quantity ?? 0;

    public int RemainingSpace =>
        IsEmpty ? 0 : ItemData.MaxStack - Quantity;

    public bool HasSpace =>
        !IsEmpty && Quantity < ItemData.MaxStack;

    public InventorySlot(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetItem(InventoryItemInstance item)
    {
        ItemInstance = item;
    }

    public void Clear()
    {
        ItemInstance = null;
    }
}