/*
 * Arquitectura: World/Runtime
 * Script: RainController
 * Rol: Presenter/controlador de lluvia basado en una fuente de tiempo desacoplada.
 * Relaciones: Consume IWorldTimeSource y publica WeatherStateEventChannel_SO cuando cambia el clima.
 * Uso como referencia: no lee campos publicos de DayNightCycle; la fuente de tiempo se asigna por Inspector.
 */
using UnityEngine;
using UnityEngine.Serialization;

public class RainController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MonoBehaviour timeSourceBehaviour;
    [FormerlySerializedAs("objetoLluvia")]
    [SerializeField] private GameObject rainObject;

    [Header("Configuracion")]
    [FormerlySerializedAs("horaInicioLluvia")]
    [SerializeField, Range(0f, 23f)] private float rainStartHour = 14f;
    [FormerlySerializedAs("duracionLluviaEnMinutos")]
    [SerializeField] private float rainDurationMinutes = 1f;

    [Header("Events")]
    [SerializeField] private WeatherStateEventChannel_SO weatherStateChannel;

    private IWorldTimeSource timeSource;
    private bool isRaining;

    private void Awake()
    {
        ResolveTimeSource();
    }

    private void OnEnable()
    {
        ResolveTimeSource();

        if (timeSource != null)
            timeSource.OnHourChanged += EvaluateRain;
    }

    private void OnDisable()
    {
        if (timeSource != null)
            timeSource.OnHourChanged -= EvaluateRain;
    }

    private void Update()
    {
        if (timeSource != null)
            EvaluateRain(timeSource.CurrentHour);
    }

    private void EvaluateRain(float currentHour)
    {
        if (rainObject == null || timeSource == null)
            return;

        float durationInWorldHours = (rainDurationMinutes / Mathf.Max(0.01f, timeSource.DayDurationMinutes)) * 24f;
        float rainEndHour = rainStartHour + durationInWorldHours;
        bool shouldRain = currentHour >= rainStartHour && currentHour <= rainEndHour;

        if (isRaining == shouldRain)
            return;

        isRaining = shouldRain;
        rainObject.SetActive(isRaining);
        weatherStateChannel?.Raise(isRaining);
    }

    private void ResolveTimeSource()
    {
        timeSource = timeSourceBehaviour as IWorldTimeSource;

        if (timeSourceBehaviour != null && timeSource == null)
            Debug.LogWarning("[RainController] El Time Source asignado no implementa IWorldTimeSource.", this);

        if (timeSource == null)
            Debug.LogWarning("[RainController] Asigna IWorldTimeSource por Inspector.", this);
    }
}
