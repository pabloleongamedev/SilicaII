using System;

public interface IInventoryReadModel
{
    int Capacity { get; }

    InventoryItemInstance GetItem(int index);

    bool CanAddItem(ItemData_SO item, int amount);

    //  EXISTENTE
    bool CanAddItemsBatch(params (ItemData_SO item, int amount)[] items);

    //  NUEVO (CLAVE PARA CRAFTING)
    bool CanProcessBatch(
        (ItemData_SO item, int amount)[] remove,
        (ItemData_SO item, int amount)[] add
    );
    int GetAmount(ItemData_SO item); 

    event Action<int, InventoryItemInstance> OnItemChanged;
}