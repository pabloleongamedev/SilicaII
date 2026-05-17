/*
 * Arquitectura: Inventory/Core
 * Script: IInventoryEvents
 * Rol: Contrato de publicacion de eventos de inventario por itemID.
 * Relaciones: InventoryEvents es la implementacion estatica legacy; adapters futuros pueden implementar este contrato para tests o escenas paralelas.
 */
public interface IInventoryEvents
{
    void PublishItemAdded(string itemID, int amount);

    void PublishItemRemoved(string itemID, int amount);
}
