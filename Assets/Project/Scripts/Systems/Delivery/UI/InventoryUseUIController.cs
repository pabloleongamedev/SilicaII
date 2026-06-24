/*
 * Arquitectura: Delivery/UI
 * Script: InventoryUseUIController
 * Rol: Presenta la accion Usar y delega el consumo en un handler activo asignado por el interactuable.
 * Relaciones: DeliveryBoxInteractable registra un IDeliveryItemUseHandler por Inspector; esta UI no conoce cajas concretas ni rutas static.
 */
using UnityEngine;

public class InventoryUseUIController : MonoBehaviour, IInventoryUseHandlerBinder
{
    [Header("Refs")]
    [SerializeField] private InventoryListView inventoryListView;
    [SerializeField] private NotificationEventChannel_SO notificationChannel;

    private IDeliveryItemUseHandler activeUseHandler;

    public void SetUseHandler(IDeliveryItemUseHandler handler)
    {
        activeUseHandler = handler;
    }

    public void ClearUseHandler(IDeliveryItemUseHandler handler)
    {
        if (activeUseHandler == handler)
            activeUseHandler = null;
    }

    public void OnUseButtonPressed()
    {
        if (inventoryListView == null)
        {
            Debug.LogError("[InventoryUseUI] InventoryListView no asignado", this);
            return;
        }

        var itemInstance = inventoryListView.GetSelectedItem();

        if (itemInstance == null)
        {
            Notify("No hay item seleccionado", NotificationType.Warning);
            return;
        }

        if (itemInstance.IsEmpty())
        {
            Notify("El item no tiene cantidad", NotificationType.Warning);
            return;
        }

        if (activeUseHandler == null)
        {
            Notify("No hay un destino activo para usar este item", NotificationType.Warning);
            return;
        }

        activeUseHandler.UseItem(itemInstance.Data, 1);
    }

    private void Notify(string msg, NotificationType type)
    {
        Debug.Log("[InventoryUseUI] " + msg, this);

        notificationChannel?.Raise(new NotificationData
        {
            message = msg,
            type = type
        });
    }
}
