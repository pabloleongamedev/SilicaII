/*
 * Arquitectura: Crafting/Events
 * Script: CraftingEvents
 * Rol: Canal de comunicacion desacoplado entre sistemas. Permite Observer/Event-driven sin referencias profundas.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;

public static class CraftingEvents
{
    public static Action<ItemData_SO, int> OnItemCrafted;
    public static Action<ItemData_SO, int> OnItemRefined;
    public static Action<NotificationData> OnNotificationRequested;
}
