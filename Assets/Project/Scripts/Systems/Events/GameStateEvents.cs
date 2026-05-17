/*
 * Arquitectura: Events/GameState
 * Script: GameStateEvents
 * Rol: Canal explicito para cambios de estado global de gameplay.
 * Relaciones: GameStateController publica aqui; sistemas que bloquean input o reaccionan a GameOver escuchan este canal.
 * Fase 2: evita que GameState viaje por el mismo bus de UI/notificaciones/input.
 */
using System;

public static class GameStateEvents
{
    public static Action<GameState> OnGameStateChanged;
}
