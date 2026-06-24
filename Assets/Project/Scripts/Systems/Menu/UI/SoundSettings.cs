/*
 * Arquitectura: Menu/UI
 * Script: SoundSettings
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Lee IGameSettingsReader/Writer desde un GameSettingsService asignable por Inspector.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Exposed Parameter Names")]
    [SerializeField] private string masterVolumeParameter = "MasterVol";
    [SerializeField] private string musicVolumeParameter = "MusicVol";
    [SerializeField] private string sfxVolumeParameter = "SFXVol";

    [Header("Settings Service")]
    [SerializeField] private MonoBehaviour settingsServiceBehaviour;

    private IGameSettingsReader settingsReader;
    private IGameSettingsWriter settingsWriter;

    private void Awake()
    {
        ResolveSettingsService();
    }

    private void Start()
    {
        ResolveSettingsService();
        ApplySound();
    }

    public void ApplySound()
    {
        if (settingsReader == null)
            return;

        SetMixerVolume(masterVolumeParameter, settingsReader.MasterVolume);
        SetMixerVolume(musicVolumeParameter, settingsReader.MusicVolume);
        SetMixerVolume(sfxVolumeParameter, settingsReader.EffectsVolume);
    }

    public void PreviewMusicVolume(float value)
    {
        SetMixerVolume(musicVolumeParameter, value);
    }

    public void PreviewSFXVolume(float value)
    {
        SetMixerVolume(sfxVolumeParameter, value);
    }

    public void PreviewMasterVolume(float segments)
    {
        segments = Mathf.Clamp(segments, 1f, 10f);
        SetMixerVolume(masterVolumeParameter, segments / 10f);
    }


    private void SetMixerVolume(string parameterName, float normalizedVolume)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("[SoundSettings] Asigna AudioMixer por Inspector para aplicar volumenes.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(parameterName))
        {
            Debug.LogWarning("[SoundSettings] Asigna el nombre del parametro expuesto del AudioMixer.", this);
            return;
        }

        normalizedVolume = Mathf.Clamp01(normalizedVolume);
        float volumeInDb = normalizedVolume <= 0.0001f ? -80f : Mathf.Log10(normalizedVolume) * 20f;

        if (!audioMixer.SetFloat(parameterName, volumeInDb))
            Debug.LogWarning($"[SoundSettings] El AudioMixer no tiene expuesto el parametro '{parameterName}'.", this);
    }

    private void ResolveSettingsService()
    {
        settingsReader = settingsServiceBehaviour as IGameSettingsReader;
        settingsWriter = settingsServiceBehaviour as IGameSettingsWriter;

        if (settingsReader == null)
            Debug.LogWarning("[SoundSettings] Asigna un GameSettingsService para aplicar volumenes desde el asset GameSettings.", this);
    }
    

}
