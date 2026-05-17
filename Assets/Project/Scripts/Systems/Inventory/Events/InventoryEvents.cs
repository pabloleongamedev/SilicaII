/*
 * Arquitectura: Inventory/Events
 * Script: InventoryEvents
 * Rol: Canal de comunicacion desacoplado entre sistemas. Permite Observer/Event-driven sin referencias profundas.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Fase desacople: los eventos por ItemData_SO quedan como compatibilidad; los eventos por itemID son la ruta runtime recomendada.
 */
using System;

public static class InventoryEvents
{
    public static Action<ItemData_SO, int> OnItemAdded;
    public static Action<ItemData_SO, int> OnItemRemoved;
    public static Action<string, int> OnItemAddedByID;
    public static Action<string, int> OnItemRemovedByID;
    public static Action<NotificationData> OnNotificationRequested;

    public static void PublishItemAdded(ItemData_SO item, int amount)
    {
        OnItemAdded?.Invoke(item, amount);

        if (item != null)
            OnItemAddedByID?.Invoke(item.itemID, amount);
    }

    public static void PublishItemRemoved(ItemData_SO item, int amount)
    {
        OnItemRemoved?.Invoke(item, amount);

        if (item != null)
            OnItemRemovedByID?.Invoke(item.itemID, amount);
    }
}
