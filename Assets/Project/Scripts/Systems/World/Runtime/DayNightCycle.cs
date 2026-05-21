/*
 * Arquitectura: World/Runtime
 * Script: DayNightCycle
 * Rol: Presenter ambiental del sol/luz. La hora canonica puede venir de WorldTimeService.
 * Relaciones: Consume IWorldTimeSource; mantiene compatibilidad local si aun no existe WorldTimeService en escena.
 * Uso como referencia: WorldTimeService decide tiempo, DayNightCycle solo presenta rotacion e intensidad.
 */
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class DayNightCycle : MonoBehaviour, IWorldTimeSource
{
    [Header("Time Source")]
    [SerializeField] private MonoBehaviour timeSourceBehaviour;
    [FormerlySerializedAs("hora")]
    [SerializeField, Range(0f, 24f)] private float fallbackHour = 9f;
    [FormerlySerializedAs("DuracionDelDiaEnMinutos")]
    [SerializeField] private float fallbackDayDurationMinutes = 24f;

    [Header("Sun Presentation")]
    [FormerlySerializedAs("sol")]
    [SerializeField] private Transform sun;

    private IWorldTimeSource timeSource;
    private Light sunLight;

    public event Action<float> OnHourChanged;

    public float CurrentHour => timeSource != null ? timeSource.CurrentHour : fallbackHour;
    public float DayDurationMinutes => timeSource != null ? timeSource.DayDurationMinutes : fallbackDayDurationMinutes;

    private void Awake()
    {
        ResolveTimeSource();

        if (sun != null)
            sunLight = sun.GetComponent<Light>();
    }

    private void OnEnable()
    {
        ResolveTimeSource();

        if (timeSource != null)
            timeSource.OnHourChanged += HandleHourChanged;
    }

    private void OnDisable()
    {
        if (timeSource != null)
            timeSource.OnHourChanged -= HandleHourChanged;
    }

    private void Update()
    {
        if (timeSource == null)
        {
            AdvanceFallback(Time.deltaTime);
            RenderSun(fallbackHour);
        }
    }

    private void HandleHourChanged(float hour)
    {
        RenderSun(hour);
        OnHourChanged?.Invoke(hour);
    }

    private void RenderSun(float hour)
    {
        if (sun == null)
            return;

        sun.localEulerAngles = new Vector3(15f * hour, 0f, 0f);

        if (sunLight != null)
            sunLight.intensity = hour < 6f || hour > 18f ? 0f : 1f;
    }

    private void AdvanceFallback(float deltaTime)
    {
        if (fallbackDayDurationMinutes <= 0f)
            return;

        fallbackHour += deltaTime * (24f / (60f * fallbackDayDurationMinutes));

        if (fallbackHour >= 24f)
            fallbackHour = 0f;

        OnHourChanged?.Invoke(fallbackHour);
    }

    private void ResolveTimeSource()
    {
        timeSource = timeSourceBehaviour as IWorldTimeSource;

        if (timeSourceBehaviour != null && timeSource == null)
            Debug.LogWarning("[DayNightCycle] El Time Source asignado no implementa IWorldTimeSource.", this);
    }
}
