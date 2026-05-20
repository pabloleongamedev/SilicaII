/*
 * Arquitectura: Events/Notification
 * Script: NotificationEvents
 * Rol: Canal explicito para feedback visual/sonoro al jugador.
 * Relaciones: Runtime publica NotificationData; NotificationManager/Audio/UI escuchan sin mezclar estado de UI o GameState.
 * Fase 2: separa notificaciones en un canal propio de dominio.
 */
using System;

public static class NotificationEvents
{
    public static Action<NotificationData> OnNotification;
    public static Action<bool> OnNotificationStateChanged;

    private static NotificationEventChannel_SO notificationChannel;
    private static NotificationStateEventChannel_SO notificationStateChannel;

    public static void ConfigureChannels(
        NotificationEventChannel_SO notificationEventChannel,
        NotificationStateEventChannel_SO notificationVisibilityChannel)
    {
        notificationChannel = notificationEventChannel;
        notificationStateChannel = notificationVisibilityChannel;
    }

    public static void PublishNotification(NotificationData notification)
    {
        OnNotification?.Invoke(notification);
        notificationChannel?.Raise(notification);
    }

    public static void PublishNotificationState(bool isVisible)
    {
        OnNotificationStateChanged?.Invoke(isVisible);
        notificationStateChannel?.Raise(isVisible);
    }
}
