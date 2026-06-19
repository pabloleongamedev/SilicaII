/*
 * Arquitectura: World/Runtime
 * Script: NightSkyStars
 * Rol: Presenter ambiental que cambia el skybox cuando llega la noche.
 * Relaciones: Consume WorldTimeService mediante IWorldTimeSource.
 * Uso como referencia: la escena asigna materiales de skybox explicitos; no crea shaders ni materiales runtime.
 */
using UnityEngine;

[DisallowMultipleComponent]
public sealed class NightSkyStars : MonoBehaviour
{
    [Header("Time Source")]
    [SerializeField] private MonoBehaviour timeSourceBehaviour;

    [Header("Schedule")]
    [SerializeField, Range(0f, 24f)] private float nightStartHour = 18f;
    [SerializeField, Range(0f, 24f)] private float nightEndHour = 6f;

    [Header("Skyboxes")]
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;

    private IWorldTimeSource timeSource;
    private bool isNightSkyboxActive;

    private void Awake()
    {
        ResolveTimeSource();
        CaptureDaySkyboxIfNeeded();
    }

    private void OnEnable()
    {
        ResolveTimeSource();
        CaptureDaySkyboxIfNeeded();
        UpdateSkybox();
    }

    private void LateUpdate()
    {
        UpdateSkybox();
    }

    private void OnDisable()
    {
        RestoreOriginalSkybox();
    }

    private void OnDestroy()
    {
        RestoreOriginalSkybox();
    }

    private void UpdateSkybox()
    {
        if (timeSource == null || nightSkybox == null)
            return;

        if (IsNight(timeSource.CurrentHour))
            ActivateNightSkybox();
        else
            RestoreOriginalSkybox();
    }

    private void ActivateNightSkybox()
    {
        CaptureDaySkyboxIfNeeded();

        if (RenderSettings.skybox == nightSkybox)
        {
            isNightSkyboxActive = true;
            return;
        }

        RenderSettings.skybox = nightSkybox;
        isNightSkyboxActive = true;
        DynamicGI.UpdateEnvironment();
    }

    private void RestoreOriginalSkybox()
    {
        if (!isNightSkyboxActive)
            return;

        if (RenderSettings.skybox == nightSkybox)
            RenderSettings.skybox = daySkybox;

        isNightSkyboxActive = false;
        DynamicGI.UpdateEnvironment();
    }

    private void CaptureDaySkyboxIfNeeded()
    {
        if (daySkybox == null && RenderSettings.skybox != nightSkybox)
            daySkybox = RenderSettings.skybox;
    }

    private bool IsNight(float hour)
    {
        return nightStartHour > nightEndHour
            ? hour >= nightStartHour || hour <= nightEndHour
            : hour >= nightStartHour && hour <= nightEndHour;
    }

    private void ResolveTimeSource()
    {
        timeSource = timeSourceBehaviour as IWorldTimeSource;

        if (timeSourceBehaviour != null && timeSource == null)
            Debug.LogWarning("[NightSkyStars] El Time Source asignado no implementa IWorldTimeSource.", this);

        if (timeSourceBehaviour == null)
            Debug.LogWarning("[NightSkyStars] Asigna WorldTimeService por Inspector.", this);

        if (nightSkybox == null)
            Debug.LogWarning("[NightSkyStars] Asigna Night Skybox por Inspector.", this);
    }
}
