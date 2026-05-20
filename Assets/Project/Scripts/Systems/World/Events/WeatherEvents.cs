/*
 * Arquitectura: World/Events
 * Script: WeatherEvents
 * Rol: Canal de dominio para cambios climaticos simples.
 * Relaciones: RainController publica lluvia activa/inactiva; audio, VFX o UI pueden escuchar sin depender del controlador.
 */
using System;

public static class WeatherEvents
{
    public static Action<bool> OnRainStateChanged;

    private static WeatherStateEventChannel_SO channel;

    public static void ConfigureChannel(WeatherStateEventChannel_SO eventChannel)
    {
        channel = eventChannel;
    }

    public static void PublishRainState(bool isRaining)
    {
        OnRainStateChanged?.Invoke(isRaining);
        channel?.Raise(isRaining);
    }
}
