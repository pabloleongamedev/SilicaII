public class InventoryItemInstance
{
    public ItemData_SO Data;

    public int Quantity;

    public bool IsStackable => Data.maxStack > 1;

    public InventoryItemInstance(ItemData_SO data)
    {
        Data = data;
        Quantity = 0;
    }

    public int Add(int amount)
    {
        int spaceLeft = Data.maxStack - Quantity;
        int added = UnityEngine.Mathf.Min(spaceLeft, amount);

        Quantity += added;
        return added;
    }

    public int Remove(int amount)
    {
        int removed = UnityEngine.Mathf.Min(amount, Quantity);
        Quantity -= removed;
        return removed;
    }

    public bool IsFull()
    {
        return Quantity >= Data.maxStack;
    }

    public bool IsEmpty()
    {
        return Quantity <= 0;
    }
    public int GetRemainingSpace()
    {
        return Data.MaxStack - Quantity;
    }
}