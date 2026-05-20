/*
 * Arquitectura: Events/EventChannels
 * Script: NotificationEventChannel_SO
 * Rol: Canal de evento serializable para notificaciones de UI/gameplay.
 * Relaciones: Salida oficial para productores de feedback y entrada para NotificationManager/NotificationAudioController.
 * Riesgo arquitectonico mitigado: evita depender de un bus estatico global en sistemas nuevos o escenas con multiples contextos.
 * Uso como referencia: los productores no conocen vistas ni audio; solo levantan NotificationData por este canal.
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
