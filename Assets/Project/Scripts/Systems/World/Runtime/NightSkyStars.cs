/*
 * Arquitectura: World/Runtime
 * Script: NightSkyStars
 * Rol: Presenter ambiental que aplica ajustes de noche sin reemplazar el skybox activo.
 * Relaciones: Consume WorldTimeService mediante IWorldTimeSource.
 * Uso como referencia: reacciona al tiempo del mundo sin buscar dependencias globales ni modificar assets compartidos.
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

    private IWorldTimeSource timeSource;

    private void Awake()
    {
        ResolveTimeSource();
    }

    private void OnEnable()
    {
        ResolveTimeSource();
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

    }
}
