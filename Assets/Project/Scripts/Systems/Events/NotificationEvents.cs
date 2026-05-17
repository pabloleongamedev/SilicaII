/*
 * Arquitectura: Events/Notification
 * Script: NotificationEvents
 * Rol: Canal explicito para feedback visual/sonoro al jugador.
 * Relaciones: Runtime publica NotificationData; NotificationManager/Audio/UI escuchan sin mezclar estado de UI o GameState.
 * Fase 2: separa notificaciones del bus transversal GameplayEvents.
 */
using System;

public static class NotificationEvents
{
    public static Action<NotificationData> OnNotification;
    public static Action<bool> OnNotificationStateChanged;
}
