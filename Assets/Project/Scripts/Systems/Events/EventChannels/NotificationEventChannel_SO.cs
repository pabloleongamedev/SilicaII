/*
 * Arquitectura: Events/EventChannels
 * Script: NotificationEventChannel_SO
 * Rol: Canal de evento serializable para notificaciones de UI/gameplay.
 * Relaciones: Puede reemplazar gradualmente NotificationEvents.OnNotification cuando un sistema quiera declarar el canal por Inspector.
 * Riesgo arquitectonico mitigado: evita depender de un bus estatico global en sistemas nuevos o escenas con multiples contextos.
 * Uso como referencia: Inventory/Crafting pueden seguir usando NotificationEvents mientras migras consumidores a EventChannels por fases.
 */
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Events/Notification Event Channel")]
public class NotificationEventChannel_SO : ScriptableObject
{
    public event Action<NotificationData> Raised;

    public void Raise(NotificationData notification)
    {
        Raised?.Invoke(notification);
    }
}
