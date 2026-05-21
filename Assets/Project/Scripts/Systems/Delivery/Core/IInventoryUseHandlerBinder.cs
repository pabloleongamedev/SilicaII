/*
 * Arquitectura: Delivery/Core
 * Script: IInventoryUseHandlerBinder
 * Rol: Contrato para que un interactuable declare quien recibira la accion Usar del inventario.
 * Relaciones: DeliveryBoxInteractable asigna el handler activo en InventoryUseUIController por Inspector.
 */
public interface IInventoryUseHandlerBinder
{
    void SetUseHandler(IDeliveryItemUseHandler handler);
    void ClearUseHandler(IDeliveryItemUseHandler handler);
}
