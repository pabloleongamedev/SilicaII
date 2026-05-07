using System;

public static class GameplayEvents
{
    // Estado global de UI (única fuente de verdad)
    public static Action<UIState> OnUIStateChanged;

    // Sistema de notificaciones (feedback jugador)
    public static Action<NotificationData> OnNotification;
    public static System.Action<bool> OnNotificationStateChanged;
    public static System.Action<GameState> OnGameStateChanged;

    public static Action OnAnyInput;
}