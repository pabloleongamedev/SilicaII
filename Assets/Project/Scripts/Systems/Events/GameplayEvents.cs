/*
 * Arquitectura: Events/GameplayEvents.cs
 * Script: GameplayEvents
 * Rol: Script de soporte del modulo. Revisar si debe separarse en Data, Core, Runtime, UI, Events o Debug.
 * Modulo: Agrupa eventos globales que aun funcionan como puente entre modulos.
 * Relaciones: Debe reducirse gradualmente a puentes claros por modulo cuando existan eventos especificos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
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