/*
 * Arquitectura: Events/UIState
 * Script: UIStateEvents
 * Rol: Canal explicito para cambios de estado de UI.
 * Relaciones: PlayerStateController publica aqui; paneles de UI escuchan este canal de dominio sin bus global.
 * Fase 2: cada dominio tiene ownership claro de sus eventos.
 */
using System;

public static class UIStateEvents
{
    public static Action<UIState> OnUIStateChanged;

    private static UIStateEventChannel_SO channel;

    public static void ConfigureChannel(UIStateEventChannel_SO eventChannel)
    {
        channel = eventChannel;
    }

    public static void Publish(UIState state)
    {
        OnUIStateChanged?.Invoke(state);
        channel?.Raise(state);
    }
}
