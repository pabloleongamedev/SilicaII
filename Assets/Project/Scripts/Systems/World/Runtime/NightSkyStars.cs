/*
 * Arquitectura: World/Runtime
 * Script: NightSkyStars
 * Rol: Presenter ambiental que cambia el skybox cuando llega la noche.
 * Relaciones: Consume WorldTimeService mediante IWorldTimeSource y puede restaurar el skybox diurno animado desde SkyboxCloudAnimator.
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
    [SerializeField] private SkyboxCloudAnimator daySkyboxAnimator;

    [Header("Sun Reset")]
    [SerializeField] private Transform sunToReset;
    [SerializeField] private bool resetSunWhileNight = true;

    private IWorldTimeSource timeSource;
    private Quaternion originalSunLocalRotation;
    private bool isNightSkyboxActive;

    private void Awake()
    {
        ResolveTimeSource();
        CaptureOriginalSunRotation();
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

        var activeNightSkybox = GetNightSkybox();
        if (RenderSettings.skybox == activeNightSkybox)
        {
            isNightSkyboxActive = true;
            ResetSunForNight();
            return;
        }

        RenderSettings.skybox = activeNightSkybox;
        isNightSkyboxActive = true;
        ResetSunForNight();
        //DynamicGI.UpdateEnvironment();
    }

    private void RestoreOriginalSkybox()
    {
        if (!isNightSkyboxActive)
            return;

        if (RenderSettings.skybox != GetDaySkybox())
            RenderSettings.skybox = GetDaySkybox();

        isNightSkyboxActive = false;
        DynamicGI.UpdateEnvironment();
    }

    private void CaptureDaySkyboxIfNeeded()
    {
        if (daySkybox == null && RenderSettings.skybox != nightSkybox)
            daySkybox = RenderSettings.skybox;
    }

    private void CaptureOriginalSunRotation()
    {
        if (sunToReset != null)
            originalSunLocalRotation = sunToReset.localRotation;
    }

    private void ResetSunForNight()
    {
        if (!resetSunWhileNight || sunToReset == null)
            return;

        sunToReset.localRotation = originalSunLocalRotation;
    }

    private Material GetDaySkybox()
    {
        if (daySkyboxAnimator != null)
        {
            var animatedSkybox = daySkyboxAnimator.GetOrCreateAnimatedSkybox(daySkybox);
            if (animatedSkybox != null)
                return animatedSkybox;
        }

        return daySkybox;
    }

    private Material GetNightSkybox()
    {
        if (daySkyboxAnimator != null)
        {
            var animatedSkybox = daySkyboxAnimator.GetOrCreateAnimatedSkybox(nightSkybox);
            if (animatedSkybox != null)
                return animatedSkybox;
        }

        return nightSkybox;
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
