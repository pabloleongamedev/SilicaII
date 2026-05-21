/*
 * Arquitectura: Delivery/Core
 * Script: IDeliveryItemUseHandler
 * Rol: Contrato para consumir un item seleccionado desde la UI sin rutas globales.
 * Relaciones: DeliveryBoxInteractable implementa este contrato; InventoryUseUIController lo invoca cuando el jugador pulsa Usar.
 */
public interface IDeliveryItemUseHandler
{
    void UseItem(ItemData_SO item, int amount);
}
