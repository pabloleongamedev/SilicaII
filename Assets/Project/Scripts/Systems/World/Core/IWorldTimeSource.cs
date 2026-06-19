/*
 * Arquitectura: World/Core
 * Script: IWorldTimeSource
 * Rol: Contrato de lectura para sistemas que necesitan hora del mundo sin depender de DayNightCycle.
 * Relaciones: WorldTimeService lo implementa; presenters como DayNightCycle, RainController y NightSkyStars lo consumen.
 */
using System;

public interface IWorldTimeSource
{
    event Action<float> OnHourChanged;

    float CurrentHour { get; }
    float DayDurationMinutes { get; }
}
