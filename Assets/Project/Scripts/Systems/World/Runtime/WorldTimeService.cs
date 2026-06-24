/*
 * Arquitectura: World/Runtime
 * Script: WorldTimeService
 * Rol: Fuente canonica de hora del mundo.
 * Relaciones: DayNightCycle observa este servicio para presentar sol/luz; RainController lo consume como IWorldTimeSource.
 * Riesgo arquitectonico mitigado: los sistemas ambientales dejan de leer campos publicos de DayNightCycle.
 */
using System;
using UnityEngine;

public class WorldTimeService : MonoBehaviour, IWorldTimeSource
{
    [SerializeField, Range(0f, 24f)] private float currentHour = 9f;
    [SerializeField] private float dayDurationMinutes = 24f;

    public event Action<float> OnHourChanged;

    public float CurrentHour => currentHour;
    public float DayDurationMinutes => dayDurationMinutes;

    private void Update()
    {
        Advance(Time.deltaTime);
    }

    public void Advance(float deltaTime)
    {
        if (dayDurationMinutes <= 0f)
            return;

        currentHour += deltaTime * (24f / (60f * dayDurationMinutes));

        if (currentHour >= 24f)
            currentHour = 0f;

        OnHourChanged?.Invoke(currentHour);
    }
}
