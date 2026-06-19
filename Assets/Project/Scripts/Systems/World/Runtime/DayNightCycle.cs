/*
 * Arquitectura: World/Runtime
 * Script: DayNightCycle
 * Rol: Presenter ambiental del sol/luz.
 * Relaciones: Consume WorldTimeService mediante IWorldTimeSource y no decide la hora del mundo.
 * Uso como referencia: WorldTimeService decide tiempo, DayNightCycle solo presenta rotacion e intensidad.
 */
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Source")]
    [SerializeField] private MonoBehaviour timeSourceBehaviour;

    [Header("Sun Presentation")]
    [SerializeField] private Transform sun;

    private IWorldTimeSource timeSource;
    private Light sunLight;

    private void Awake()
    {
        if (sun != null)
            sunLight = sun.GetComponent<Light>();
    }

    private void OnEnable()
    {
        ResolveTimeSource();

        if (timeSource != null)
        {
            timeSource.OnHourChanged += HandleHourChanged;
            RenderSun(timeSource.CurrentHour);
        }
    }

    private void OnDisable()
    {
        if (timeSource != null)
            timeSource.OnHourChanged -= HandleHourChanged;
    }

    private void HandleHourChanged(float hour)
    {
        RenderSun(hour);
    }

    private void RenderSun(float hour)
    {
        if (sun == null)
            return;

        sun.localEulerAngles = new Vector3(15f * hour, 0f, 0f);

        if (sunLight != null)
            sunLight.intensity = hour < 6f || hour > 18f ? 0f : 1f;
    }

    private void ResolveTimeSource()
    {
        timeSource = timeSourceBehaviour as IWorldTimeSource;

        if (timeSourceBehaviour != null && timeSource == null)
            Debug.LogWarning("[DayNightCycle] El Time Source asignado no implementa IWorldTimeSource.", this);

        if (timeSourceBehaviour == null)
            Debug.LogWarning("[DayNightCycle] Asigna WorldTimeService por Inspector.", this);
    }
}
