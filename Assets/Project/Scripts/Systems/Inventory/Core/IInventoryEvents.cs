/*
 * Arquitectura: Inventory/Core
 * Script: IInventoryEvents
 * Rol: Contrato de publicacion de eventos de inventario por itemID.
 * Relaciones: InventoryEventChannel_SO es la salida oficial en escena; este contrato queda disponible para adapters de tests o servicios puros.
 */
public interface IInventoryEvents
{
    void PublishItemAdded(string itemID, int amount);

    void PublishItemRemoved(string itemID, int amount);
}
