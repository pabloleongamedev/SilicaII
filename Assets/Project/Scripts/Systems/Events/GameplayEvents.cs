/*
 * Arquitectura: Events
 * Script: GameplayEvents
 * Rol: Fachada legacy de compatibilidad para codigo o escenas que aun no migraron a eventos por dominio.
 * Modulo: Delegar nuevos desarrollos a UIStateEvents, NotificationEvents, GameStateEvents e InputActivityEvents.
 * Relaciones: No debe recibir nuevos consumidores; existe para reducir riesgo mientras se migran referencias viejas.
 * Fase 2: el bus global queda documentado como deprecated y los canales nuevos tienen ownership explicito.
 */
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
