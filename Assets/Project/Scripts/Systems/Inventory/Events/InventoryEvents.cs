/*
 * Arquitectura: Inventory/Events
 * Script: InventoryEvents
 * Rol: Canal de comunicacion desacoplado entre sistemas. Permite Observer/Event-driven sin referencias profundas.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;

public static class InventoryEvents
{
    public static Action<ItemData_SO, int> OnItemAdded;
    public static Action<ItemData_SO, int> OnItemRemoved;
    public static Action<NotificationData> OnNotificationRequested;
}
