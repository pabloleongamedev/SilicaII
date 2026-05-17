/*
 * Arquitectura: Events/UIState
 * Script: UIStateEvents
 * Rol: Canal explicito para cambios de estado de UI.
 * Relaciones: PlayerStateController publica aqui; paneles de UI escuchan este canal sin depender del bus global GameplayEvents.
 * Fase 2: reemplaza el subconjunto UI de GameplayEvents para que cada dominio tenga ownership claro.
 */
using System;

public static class UIStateEvents
{
    public static Action<UIState> OnUIStateChanged;
}
