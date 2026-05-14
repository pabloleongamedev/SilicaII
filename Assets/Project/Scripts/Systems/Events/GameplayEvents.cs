/*
 * Arquitectura: Events
 * Script: GameplayEvents
 * Rol: Bus global temporal para UI state, notificaciones, estado de juego e input.
 * Modulo: Agrupa eventos transversales que todavia no tienen ownership por sistema.
 * Relaciones: PlayerStateController publica UIState; UI/Crafting/Inventory/Delivery/Teleport publican notificaciones; NotificationManager escucha feedback.
 * Riesgo arquitectonico: mezcla varios dominios en un unico bus estatico; debe separarse en UIStateEvents, NotificationEvents e InputActivityEvents o servicios runtime.
 * Uso como referencia: mantenerlo como compatibilidad mientras se crean canales con responsabilidad explicita.
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
