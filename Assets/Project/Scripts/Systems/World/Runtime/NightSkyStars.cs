/*
 * Arquitectura: World/Runtime
 * Script: NightSkyStars
 * Rol: Presenter ambiental que actualiza la fase nocturna del skybox activo sin cambiar de material.
 * Relaciones: Consume WorldTimeService mediante IWorldTimeSource.
 * Uso como referencia: reacciona al tiempo del mundo y delega la apariencia dia/noche a la paleta del material.
 */
using UnityEngine;

[DisallowMultipleComponent]
public sealed class NightSkyStars : MonoBehaviour
{
    private static readonly int NightPhaseId = Shader.PropertyToID("_NightPhase");
    private static readonly int UseNightPaletteId = Shader.PropertyToID("_UseNightPalette");

    [Header("Time Source")]
    [SerializeField] private MonoBehaviour timeSourceBehaviour;

    [Header("Schedule")]
    [SerializeField, Range(0f, 24f)] private float nightStartHour = 18f;
    [SerializeField, Range(0f, 24f)] private float midnightHour = 0f;
    [SerializeField, Range(0f, 24f)] private float nightEndHour = 6f;

    [Header("Skybox Palette")]
    [SerializeField] private Material skyboxMaterial;

    private IWorldTimeSource timeSource;

    private void Awake()
    {
        ResolveTimeSource();
    }

    private void OnEnable()
    {
        ResolveTimeSource();
        UpdateNightPhase();
    }

    private void LateUpdate()
    {
        UpdateNightPhase();
    }

    private void UpdateNightPhase()
    {
        if (timeSource == null)
            return;

        var material = skyboxMaterial != null ? skyboxMaterial : RenderSettings.skybox;
        if (material == null)
            return;

        bool isNight = IsNight(timeSource.CurrentHour);

        if (material.HasProperty(NightPhaseId))
            material.SetFloat(NightPhaseId, isNight ? CalculateNightPhase(timeSource.CurrentHour) : 0f);

        if (material.HasProperty(UseNightPaletteId))
            material.SetFloat(UseNightPaletteId, isNight ? 1f : 0f);
    }

    private float CalculateNightPhase(float hour)
    {
        float nightStart = NormalizeHour(nightStartHour);
        float midnight = NormalizePhaseAfter(midnightHour, nightStart);
        float nightEnd = NormalizePhaseAfter(nightEndHour, midnight);
        float current = NormalizeHour(hour);

        if (current < nightStart)
            current += 24f;

        if (current <= midnight)
            return Mathf.Lerp(0f, 0.5f, Mathf.InverseLerp(nightStart, midnight, current));

        return Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(midnight, nightEnd, current));
    }

    private bool IsNight(float hour)
    {
        return nightStartHour > nightEndHour
            ? hour >= nightStartHour || hour <= nightEndHour
            : hour >= nightStartHour && hour <= nightEndHour;
    }

    private static float NormalizeHour(float hour)
    {
        return Mathf.Repeat(hour, 24f);
    }

    private static float NormalizePhaseAfter(float hour, float previous)
    {
        float normalized = NormalizeHour(hour);
        while (normalized <= previous)
            normalized += 24f;

        return normalized;
    }

    private void ResolveTimeSource()
    {
        timeSource = timeSourceBehaviour as IWorldTimeSource;

        if (timeSourceBehaviour != null && timeSource == null)
            Debug.LogWarning("[NightSkyStars] El Time Source asignado no implementa IWorldTimeSource.", this);

        if (timeSourceBehaviour == null)
            Debug.LogWarning("[NightSkyStars] Asigna WorldTimeService por Inspector.", this);

        if (skyboxMaterial == null)
            Debug.LogWarning("[NightSkyStars] Asigna TSI_Skybox_01A como Skybox Material por Inspector.", this);
    }
}
