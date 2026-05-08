/*
 * Arquitectura: Inventory/Core
 * Script: InventoryItemInstance
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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