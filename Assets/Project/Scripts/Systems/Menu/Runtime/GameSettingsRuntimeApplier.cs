/*
 * Arquitectura: Menu/Runtime
 * Script: GameSettingsRuntimeApplier
 * Rol: Aplica preferencias persistidas al arrancar una escena runtime.
 * Relaciones: Lee IGameSettingsReader/Writer por Inspector y aplica pantalla/audio sin depender de singletons.
 * Riesgo arquitectonico mitigado: cada escena declara como consume configuracion persistente; no hay bus global ni busqueda oculta.
 */
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettingsRuntimeApplier : MonoBehaviour
{
    private const float NeutralBrightness = 0.6f;

    [Header("Settings Service")]
    [SerializeField] private MonoBehaviour settingsServiceBehaviour;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolumeParameter = "MasterVol";
    [SerializeField] private string musicVolumeParameter = "MusicVol";
    [SerializeField] private string sfxVolumeParameter = "SFXVol";

    [Header("Optional Brightness Overlay")]
    [SerializeField] private Image brightnessOverlay;

    [Header("Optional Post Processing")]
    [SerializeField] private MonoBehaviour globalVolume;
    [SerializeField] private float minPostExposure = -1f;
    [SerializeField] private float maxPostExposure = 0.1f;

    private IGameSettingsReader settingsReader;
    private IGameSettingsWriter settingsWriter;

    private void Awake()
    {
        ResolveSettingsService();

        if (settingsWriter != null)
            settingsWriter.Load();
    }

    private void Start()
    {
        ApplySettings();
    }

    public void ApplySettings()
    {
        if (settingsReader == null)
            return;

        Screen.fullScreen = settingsReader.Fullscreen;
        ApplyBrightness(settingsReader.Brightness);
        ApplyVolume(masterVolumeParameter, settingsReader.MasterVolume);
        ApplyVolume(musicVolumeParameter, settingsReader.MusicVolume);
        ApplyVolume(sfxVolumeParameter, settingsReader.EffectsVolume);
    }

    private void ApplyBrightness(float value)
    {
        if (brightnessOverlay == null)
        {
            ApplyPostExposure(value);
            return;
        }

        Color color = brightnessOverlay.color;
        color.a = 1f - Mathf.Clamp01(value);
        brightnessOverlay.color = color;
        ApplyPostExposure(value);
    }

    private void ApplyPostExposure(float value)
    {
        if (globalVolume == null)
            return;

        object profile = GetFieldOrPropertyValue(globalVolume, "profile")
            ?? GetFieldOrPropertyValue(globalVolume, "sharedProfile");
        if (profile == null)
        {
            Debug.LogWarning("[GameSettingsRuntimeApplier] El Global Volume asignado no expone profile/sharedProfile.", this);
            return;
        }

        if (!TryGetColorAdjustments(profile, out object colorAdjustments))
        {
            Debug.LogWarning("[GameSettingsRuntimeApplier] El VolumeProfile asignado no contiene ColorAdjustments.", this);
            return;
        }

        object postExposure = GetFieldOrPropertyValue(colorAdjustments, "postExposure");
        if (postExposure == null)
            return;

        SetFieldOrPropertyValue(postExposure, "overrideState", true);
        SetFieldOrPropertyValue(postExposure, "value", GetPostExposure(value));
    }

    private float GetPostExposure(float value)
    {
        float normalized = Mathf.Clamp01(value);

        if (normalized < NeutralBrightness)
            return Mathf.Lerp(minPostExposure, 0f, normalized / NeutralBrightness);

        return Mathf.Lerp(0f, maxPostExposure, (normalized - NeutralBrightness) / (1f - NeutralBrightness));
    }

    private bool TryGetColorAdjustments(object profile, out object colorAdjustments)
    {
        colorAdjustments = null;

        var components = GetFieldOrPropertyValue(profile, "components") as System.Collections.IEnumerable;
        if (components == null)
            return false;

        foreach (object component in components)
        {
            if (component == null)
                continue;

            if (component.GetType().Name == "ColorAdjustments")
            {
                colorAdjustments = component;
                return true;
            }
        }

        return false;
    }

    private object GetFieldOrPropertyValue(object target, string memberName)
    {
        var type = target.GetType();
        var field = type.GetField(memberName);
        if (field != null)
            return field.GetValue(target);

        return type.GetProperty(memberName)?.GetValue(target);
    }

    private void SetFieldOrPropertyValue(object target, string memberName, object value)
    {
        var type = target.GetType();
        var field = type.GetField(memberName);
        if (field != null)
        {
            field.SetValue(target, value);
            return;
        }

        var property = type.GetProperty(memberName);
        if (property != null && property.CanWrite)
            property.SetValue(target, value);
    }

    private void ApplyVolume(string parameterName, float normalizedVolume)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("[GameSettingsRuntimeApplier] Asigna AudioMixer por Inspector para aplicar preferencias de audio.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(parameterName))
        {
            Debug.LogWarning("[GameSettingsRuntimeApplier] Asigna nombres de parametros expuestos del AudioMixer.", this);
            return;
        }

        normalizedVolume = Mathf.Clamp01(normalizedVolume);
        float volumeInDb = normalizedVolume <= 0.0001f ? -80f : Mathf.Log10(normalizedVolume) * 20f;

        if (!audioMixer.SetFloat(parameterName, volumeInDb))
            Debug.LogWarning($"[GameSettingsRuntimeApplier] El AudioMixer no tiene expuesto el parametro '{parameterName}'.", this);
    }

    private void ResolveSettingsService()
    {
        settingsReader = settingsServiceBehaviour as IGameSettingsReader;
        settingsWriter = settingsServiceBehaviour as IGameSettingsWriter;

        if (settingsReader == null)
            Debug.LogWarning("[GameSettingsRuntimeApplier] Asigna un GameSettingsService por Inspector.", this);
    }
}
