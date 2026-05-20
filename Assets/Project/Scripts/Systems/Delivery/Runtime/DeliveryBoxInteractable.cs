/*
 * Arquitectura: Delivery/Runtime
 * Script: DeliveryBoxInteractable
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona la entrega/uso de items desde la UI de inventario hacia objetos interactuables.
 * Relaciones: Usa Inventory y PlayerState para seleccionar y consumir items entregados.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class DeliveryBoxInteractable : MonoBehaviour, IInteractable
{
    [Header("Config")]
    [SerializeField] private int amountToConsume = 1;

    [Header("Refs")]
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private PlayerStateController playerState;
    [SerializeField] private NotificationEventChannel_SO notificationChannel;

    private IInventoryReadModel read;
    private IInventoryWriteModel write;

    // 🔥 referencia ACTIVA (sin singleton global sucio)
    private static DeliveryBoxInteractable activeBox;

    private bool isWaitingForItem = false;

    // =========================================================
    private void Start()
    {
        read = inventoryController.ReadModel;
        write = inventoryController.WriteModel;
    }

    // =========================================================
    public void Interact(InteractionContext context)
    {
        if (IsInventoryEmpty())
        {
            Notify("Ve a buscar muchos elementos", NotificationType.Warning);
            return;
        }

        // 🔥 ACTIVAR ESTA CAJA
        activeBox = this;
        isWaitingForItem = true;

        playerState.SetState(UIState.Inventory);

        Notify("Selecciona un objeto para entregar", NotificationType.Info);
    }

    public string GetInteractionText()
    {
        return "Presiona E para usar la caja";
    }

    // =========================================================
    // 🔥 BOTÓN "USAR" LLAMA A ESTO
    // =========================================================
    public static void OnUseButtonPressed(ItemData_SO item, int amount)
    {
        if (activeBox == null)
            return;

        activeBox.HandleUse(item, amount);
    }

    // =========================================================
    private void HandleUse(ItemData_SO item, int amount)
    {
        if (!isWaitingForItem)
            return;

        if (item == null)
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

        // 🔥 LIMPIAR ESTADO
        isWaitingForItem = false;
        activeBox = null;

        playerState.SetState(UIState.None);
    }

    // =========================================================
    private bool IsInventoryEmpty()
    {
        for (int i = 0; i < read.Capacity; i++)
        {
            var slot = read.GetItem(i);

            if (slot != null && slot.Quantity > 0)
                return false;
        }

        return true;
    }

    // =========================================================
    private void Notify(string msg, NotificationType type)
    {
        Debug.Log("[DeliveryBox] " + msg);

        notificationChannel?.Raise(new NotificationData
        {
            message = msg,
            type = type
        });
    }
}
