/*
 * Arquitectura: Crafting/Events
 * Script: CraftingEvents
 * Rol: Canal de comunicacion desacoplado entre sistemas. Permite Observer/Event-driven sin referencias profundas.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Fase desacople: los eventos por ItemData_SO quedan como compatibilidad; los eventos por itemID son la ruta runtime recomendada.
 */
using System;

public static class CraftingEvents
{
    public static Action<ItemData_SO, int> OnItemCrafted;
    public static Action<ItemData_SO, int> OnItemRefined;
    public static Action<string, int> OnItemCraftedByID;
    public static Action<string, int> OnItemRefinedByID;
    public static Action<NotificationData> OnNotificationRequested;
    private static CraftingEventChannel_SO channel;

    public static void ConfigureChannel(CraftingEventChannel_SO eventChannel)
    {
        channel = eventChannel;
    }

    public static void PublishItemCrafted(ItemData_SO item, int amount)
    {
        OnItemCrafted?.Invoke(item, amount);
        channel?.RaiseItemCrafted(item, amount);

        if (item != null)
        {
            OnItemCraftedByID?.Invoke(item.itemID, amount);
            channel?.RaiseItemCraftedByID(item.itemID, amount);
        }
    }

    public static void PublishItemRefined(ItemData_SO item, int amount)
    {
        OnItemRefined?.Invoke(item, amount);
        channel?.RaiseItemRefined(item, amount);

        if (item != null)
        {
            OnItemRefinedByID?.Invoke(item.itemID, amount);
            channel?.RaiseItemRefinedByID(item.itemID, amount);
        }
    }

    public static void PublishNotificationRequested(NotificationData notification)
    {
        OnNotificationRequested?.Invoke(notification);
        channel?.RaiseNotification(notification);
    }
}
