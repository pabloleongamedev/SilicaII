/*
 * Arquitectura: World/Core
 * Script: IWorldTimeSource
 * Rol: Contrato de lectura para sistemas que necesitan hora del mundo sin depender de DayNightCycle.
 * Relaciones: WorldTimeService y DayNightCycle pueden implementarlo; RainController consume este contrato.
 */
using System;

public interface IWorldTimeSource
{
    event Action<float> OnHourChanged;

    float CurrentHour { get; }
    float DayDurationMinutes { get; }
}
