/*
 * Arquitectura: Inventory/Core
 * Script: InventorySlot
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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