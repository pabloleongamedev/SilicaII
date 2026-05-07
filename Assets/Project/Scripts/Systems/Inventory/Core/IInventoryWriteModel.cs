public interface IInventoryWriteModel
{
    int AddItem(ItemData_SO data, int amount);
    void MoveItem(int fromIndex, int toIndex);
    void RemoveItem(ItemData_SO item, int amount);
    bool TryProcessBatch(
        (ItemData_SO item, int amount)[] remove,
        (ItemData_SO item, int amount)[] add
    );
    void Clear();
}
