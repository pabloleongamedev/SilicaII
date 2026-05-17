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

        DebugSetMixerVolume(masterVolumeParameter, settingsReader.MasterVolume);
        DebugSetMixerVolume(musicVolumeParameter, settingsReader.MusicVolume);
        DebugSetMixerVolume(sfxVolumeParameter, settingsReader.EffectsVolume);
    }

    public void PreviewMusicVolume(float value)
    {
        DebugSetMixerVolume(musicVolumeParameter, value);
    }

    public void PreviewSFXVolume(float value)
    {
        DebugSetMixerVolume(sfxVolumeParameter, value);
    }

    public void PreviewMasterVolume(float segments)
{
    segments = Mathf.Clamp(segments, 1f, 10f);

    float normalizedVolume = segments / 10f;
    if (settingsWriter != null)
        settingsWriter.MasterVolume = normalizedVolume;

    DebugSetMixerVolume(masterVolumeParameter, normalizedVolume);
}


    private void DebugSetMixerVolume(string parameterName, float normalizedVolume)
    {
        normalizedVolume = Mathf.Clamp(normalizedVolume, 0.5f, 1f);
        float volumeInDb = Mathf.Log10(normalizedVolume) * 20f;

        bool setOk = audioMixer.SetFloat(parameterName, volumeInDb);

        float currentValue;
        bool getOk = audioMixer.GetFloat(parameterName, out currentValue);

        Debug.Log(
            $"[AUDIO DEBUG] Param: {parameterName} | Normalized: {normalizedVolume} | dB: {volumeInDb} | SetOK: {setOk} | GetOK: {getOk} | Current: {currentValue}"
        );
    }

    private void ResolveSettingsService()
    {
        if (settingsServiceBehaviour == null)
            settingsServiceBehaviour = GetComponentInParent<GameSettingsService>();

        if (settingsServiceBehaviour == null)
            settingsServiceBehaviour = gameObject.AddComponent<GameSettingsService>();

        settingsReader = settingsServiceBehaviour as IGameSettingsReader;
        settingsWriter = settingsServiceBehaviour as IGameSettingsWriter;

        if (settingsReader == null)
            Debug.LogWarning("[SoundSettings] Asigna un GameSettingsService para aplicar volumenes sin usar GameSettings.Instance.", this);
    }
    

}
