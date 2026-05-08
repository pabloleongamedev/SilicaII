/// <summary>
/// Contiene dependencias necesarias para interactuar.
/// Evita usar GetComponent dentro de cada interactuable.
/// </summary>
public class InteractionContext
{
    public IInventoryReadModel InventoryRead { get; private set; }
    public IInventoryWriteModel InventoryWrite { get; private set; }

    public InteractionContext(IInventoryReadModel inventoryRead, IInventoryWriteModel inventoryWrite)
    {
        InventoryRead = inventoryRead;
        InventoryWrite = inventoryWrite;
    }

    public InteractionContext(InventorySystem inventory)
        : this(inventory.ReadModel, inventory)
    {
    }
}
