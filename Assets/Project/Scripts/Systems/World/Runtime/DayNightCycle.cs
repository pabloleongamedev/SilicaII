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

    [SerializeField, Range(0f, 24f)] private float zenithHour = 12f;
    [SerializeField, Range(0f, 24f)] private float sunsetHour = 17f;

    [FormerlySerializedAs("duskHour")]
    [SerializeField, Range(0f, 24f)] private float nightStartHour = 18f;

    [SerializeField, Range(0f, 24f)] private float midnightHour = 0f;
    [SerializeField, Range(0f, 24f)] private float nightEndHour = 5.5f;

    [Header("Sun Presentation")]
    [SerializeField] private Transform sun;

    [FormerlySerializedAs("sunriseEulerAngles")]
    [SerializeField] private Vector3 dawnSunEulerAngles = new Vector3(90f, 0f, 0f);

    [SerializeField] private Vector3 zenithSunEulerAngles = new Vector3(180f, 0f, 0f);

    [FormerlySerializedAs("duskSunEulerAngles")]
    [SerializeField] private Vector3 sunsetSunEulerAngles = new Vector3(250f, 0f, 0f);

    [SerializeField] private Vector3 nightStartSunEulerAngles = new Vector3(270f, 0f, 0f);
    [SerializeField] private Vector3 midnightSunEulerAngles = new Vector3(360f, 0f, 0f);
    [SerializeField] private Vector3 nightEndSunEulerAngles = new Vector3(75f, 0f, 0f);

    [Header("Light Intensity")]
    [SerializeField] private float dawnSunIntensity = 0.45f;
    [SerializeField] private float zenithSunIntensity = 1f;
    [SerializeField] private float sunsetSunIntensity = 0.55f;
    [SerializeField] private float nightStartSunIntensity = 0.2f;
    [SerializeField] private float midnightSunIntensity = 0.12f;
    [SerializeField] private float nightEndSunIntensity = 0.25f;

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
            sunLight.intensity = CalculateSunIntensity(hour);
    }

    private Vector3 CalculateSunEulerAngles(float hour)
    {
        float current = NormalizeHourForCycle(hour);
        float dawn = NormalizeHourForCycle(dawnHour);
        float zenith = NormalizePhaseAfter(zenithHour, dawn);
        float sunset = NormalizePhaseAfter(sunsetHour, zenith);
        float nightStart = NormalizePhaseAfter(nightStartHour, sunset);
        float midnight = NormalizePhaseAfter(midnightHour, nightStart);
        float nightEnd = NormalizePhaseAfter(nightEndHour, midnight);
        float nextDawn = dawn + 24f;

        if (current < dawn)
            current += 24f;

        if (current <= zenith)
            return LerpEulerAngles(dawnSunEulerAngles, zenithSunEulerAngles, Mathf.InverseLerp(dawn, zenith, current));

        if (current <= sunset)
            return LerpEulerAngles(zenithSunEulerAngles, sunsetSunEulerAngles, Mathf.InverseLerp(zenith, sunset, current));

        if (current <= nightStart)
            return LerpEulerAngles(sunsetSunEulerAngles, nightStartSunEulerAngles, Mathf.InverseLerp(sunset, nightStart, current));

        if (current <= midnight)
            return LerpEulerAngles(nightStartSunEulerAngles, sunsetSunEulerAngles, Mathf.InverseLerp(nightStart, midnight, current));

        if (current <= nightEnd)
            return LerpEulerAngles(sunsetSunEulerAngles, zenithSunEulerAngles, Mathf.InverseLerp(midnight, nightEnd, current));

        return LerpEulerAngles(zenithSunEulerAngles, dawnSunEulerAngles, Mathf.InverseLerp(nightEnd, nextDawn, current));
    }

    private float CalculateSunIntensity(float hour)
    {
        float current = NormalizeHourForCycle(hour);
        float dawn = NormalizeHourForCycle(dawnHour);
        float zenith = NormalizePhaseAfter(zenithHour, dawn);
        float sunset = NormalizePhaseAfter(sunsetHour, zenith);
        float nightStart = NormalizePhaseAfter(nightStartHour, sunset);
        float midnight = NormalizePhaseAfter(midnightHour, nightStart);
        float nightEnd = NormalizePhaseAfter(nightEndHour, midnight);
        float nextDawn = dawn + 24f;

        if (current < dawn)
            current += 24f;

        if (current <= zenith)
            return Mathf.Lerp(dawnSunIntensity, zenithSunIntensity, Mathf.InverseLerp(dawn, zenith, current));

        if (current <= sunset)
            return Mathf.Lerp(zenithSunIntensity, sunsetSunIntensity, Mathf.InverseLerp(zenith, sunset, current));

        if (current <= nightStart)
            return Mathf.Lerp(sunsetSunIntensity, nightStartSunIntensity, Mathf.InverseLerp(sunset, nightStart, current));

        if (current <= midnight)
            return Mathf.Lerp(nightStartSunIntensity, midnightSunIntensity, Mathf.InverseLerp(nightStart, midnight, current));

        if (current <= nightEnd)
            return Mathf.Lerp(midnightSunIntensity, nightEndSunIntensity, Mathf.InverseLerp(midnight, nightEnd, current));

        return Mathf.Lerp(nightEndSunIntensity, dawnSunIntensity, Mathf.InverseLerp(nightEnd, nextDawn, current));
    }

    private float NormalizeHourForCycle(float hour)
    {
        float normalized = Mathf.Repeat(hour, 24f);
        if (normalized < dawnHour)
            normalized += 24f;

        return normalized;
    }

    private static float NormalizePhaseAfter(float hour, float previous)
    {
        float normalized = Mathf.Repeat(hour, 24f);
        while (normalized <= previous)
            normalized += 24f;

        return normalized;
    }

    private static Vector3 LerpEulerAngles(Vector3 from, Vector3 to, float progress)
    {
        return new Vector3(
            Mathf.LerpAngle(from.x, to.x, progress),
            Mathf.LerpAngle(from.y, to.y, progress),
            Mathf.LerpAngle(from.z, to.z, progress));
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
