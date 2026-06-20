/*
 * Arquitectura: World/Runtime
 * Script: DayNightCycle
 * Rol: Presenter ambiental del sol/luz.
 * Relaciones: Consume WorldTimeService mediante IWorldTimeSource y no decide la hora del mundo.
 * Uso como referencia: WorldTimeService decide tiempo, DayNightCycle solo presenta rotacion e intensidad.
 */
using UnityEngine;
using UnityEngine.Serialization;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Source")]
    [SerializeField] private MonoBehaviour timeSourceBehaviour;

    [Header("Schedule")]
    [FormerlySerializedAs("dayStartHour")]
    [SerializeField, Range(0f, 24f)] private float dawnHour = 6f;

    [FormerlySerializedAs("nightStartHour")]
    [SerializeField, Range(0f, 24f)] private float duskHour = 18f;

    [Header("Sun Presentation")]
    [SerializeField] private Transform sun;
    [SerializeField] private float daySunIntensity = 1f;
    [SerializeField] private float nightSunIntensity = 0.2f;

    [FormerlySerializedAs("sunriseEulerAngles")]
    [SerializeField] private Vector3 dawnSunEulerAngles = new Vector3(90f, 0f, 0f);

    [FormerlySerializedAs("sunsetEulerAngles")]
    [SerializeField] private Vector3 duskSunEulerAngles = new Vector3(270f, 0f, 0f);

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

        sun.localEulerAngles = CalculateSunEulerAngles(hour);

        if (sunLight != null)
            sunLight.intensity = IsDay(hour) ? daySunIntensity : nightSunIntensity;
    }

    private Vector3 CalculateSunEulerAngles(float hour)
    {
        if (IsDay(hour))
        {
            float dayProgress = Mathf.InverseLerp(dawnHour, duskHour, hour);
            return Vector3.Lerp(dawnSunEulerAngles, duskSunEulerAngles, dayProgress);
        }

        float nightProgress = hour >= duskHour
            ? Mathf.InverseLerp(duskHour, 24f, hour) * 0.5f
            : 0.5f + Mathf.InverseLerp(0f, dawnHour, hour) * 0.5f;

        return Vector3.Lerp(duskSunEulerAngles, dawnSunEulerAngles, nightProgress);
    }

    private bool IsDay(float hour)
    {
        return hour >= dawnHour && hour <= duskHour;
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
