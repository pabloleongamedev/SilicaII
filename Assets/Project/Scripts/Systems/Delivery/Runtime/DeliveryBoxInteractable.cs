/*
 * Arquitectura: Delivery/Runtime
 * Script: DeliveryBoxInteractable
 * Rol: Interactable de entrega. Activa la UI de inventario y recibe el item elegido mediante contrato.
 * Relaciones: Usa Inventory, PlayerState e InventoryUseUIController asignados por Inspector; no mantiene rutas globales ni static state.
 */
using UnityEngine;

public class DeliveryBoxInteractable : MonoBehaviour, IInteractable, IDeliveryItemUseHandler
{
    [Header("Config")]
    [SerializeField] private int amountToConsume = 1;

    [Header("Refs")]
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private PlayerStateController playerState;
    [SerializeField] private MonoBehaviour inventoryUseHandlerBinderBehaviour;
    [SerializeField] private NotificationEventChannel_SO notificationChannel;

    private IInventoryReadModel read;
    private IInventoryWriteModel write;
    private IInventoryUseHandlerBinder inventoryUseHandlerBinder;
    private bool isWaitingForItem;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Start()
    {
        ResolveReferences();
    }

    public void Interact(InteractionContext context)
    {
        ResolveReferences();

        if (read == null || write == null || playerState == null || inventoryUseHandlerBinder == null)
        {
            Notify("Faltan referencias para entregar items", NotificationType.Warning);
            return;
        }

        if (IsInventoryEmpty())
        {
            Notify("Ve a buscar muchos elementos", NotificationType.Warning);
            return;
        }

        isWaitingForItem = true;
        inventoryUseHandlerBinder.SetUseHandler(this);
        playerState.SetState(UIState.Inventory);
        Notify("Selecciona un objeto para entregar", NotificationType.Info);
    }

    public string GetInteractionText()
    {
        return "Presiona E para usar la caja";
    }

    public void UseItem(ItemData_SO item, int amount)
    {
        if (!isWaitingForItem)
            return;

        if (item == null || read == null || write == null)
            return;

        int available = read.GetAmount(item);

        if (available <= 0)
        {
            Notify("No tienes ese objeto", NotificationType.Warning);
            return;
        }

        int toRemove = Mathf.Min(amountToConsume, available);
        write.RemoveItem(item, toRemove);
        Notify($"Has entregado {item.name} x{toRemove}", NotificationType.Success);

        ClearWaitingState();

        if (playerState != null)
            playerState.SetState(UIState.None);
    }

    private bool IsInventoryEmpty()
    {
        if (read == null)
            return true;

        for (int i = 0; i < read.Capacity; i++)
        {
            var slot = read.GetItem(i);

            if (slot != null && slot.Quantity > 0)
                return false;
        }

        return true;
    }

    private void ResolveReferences()
    {
        if (inventoryController == null)
        {
            Debug.LogWarning("[DeliveryBoxInteractable] Asigna InventoryController por Inspector.", this);
        }
        else
        {
            read = inventoryController.ReadModel;
            write = inventoryController.WriteModel;
        }

        if (playerState == null)
            Debug.LogWarning("[DeliveryBoxInteractable] Asigna PlayerStateController por Inspector.", this);

        inventoryUseHandlerBinder = inventoryUseHandlerBinderBehaviour as IInventoryUseHandlerBinder;

        if (inventoryUseHandlerBinder == null)
            Debug.LogWarning("[DeliveryBoxInteractable] Asigna InventoryUseUIController por Inspector.", this);
    }

    private void ClearWaitingState()
    {
        isWaitingForItem = false;
        inventoryUseHandlerBinder?.ClearUseHandler(this);
    }

    private void Notify(string msg, NotificationType type)
    {
        Debug.Log("[DeliveryBox] " + msg, this);

        notificationChannel?.Raise(new NotificationData
        {
            message = msg,
            type = type
        });
    }
}
