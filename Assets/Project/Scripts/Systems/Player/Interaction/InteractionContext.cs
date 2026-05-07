using UnityEngine;

/// <summary>
/// Contiene dependencias necesarias para interactuar.
/// Evita usar GetComponent dentro de cada interactuable.
/// </summary>
public class InteractionContext
{
    public IInventoryReadModel InventoryRead { get; private set; }
    public IInventoryWriteModel InventoryWrite { get; private set; }

    public InteractionContext(InventorySystem inventory)
    {
        InventoryRead = inventory.ReadModel;
        InventoryWrite = inventory;
    }
}