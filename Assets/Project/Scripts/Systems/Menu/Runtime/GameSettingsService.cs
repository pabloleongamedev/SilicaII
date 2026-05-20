/*
 * Arquitectura: Menu/Runtime
 * Script: GameSettingsService
 * Rol: Adapter de escena para configuracion persistente.
 * Relaciones: Implementa IGameSettingsReader/IGameSettingsWriter y usa GameSettings como asset/config asignado por Inspector.
 * Riesgo arquitectonico mitigado: UI y Audio dependen de contratos asignables; no existe singleton global de configuracion.
 * Uso como referencia: si luego cambias PlayerPrefs por archivo, nube o perfil de usuario, los consumidores no cambian.
 */
using UnityEngine;

public class GameSettingsService : MonoBehaviour, IGameSettingsReader, IGameSettingsWriter
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private bool loadOnAwake = true;

    public float Brightness
    {
        get => Settings.Brightness;
        set => Settings.Brightness = value;
    }

    public float MusicVolume
    {
        get => Settings.MusicVolume;
        set => Settings.MusicVolume = value;
    }

    public float EffectsVolume
    {
        get => Settings.EffectsVolume;
        set => Settings.EffectsVolume = value;
    }

    public float MasterVolume
    {
        get => Settings.MasterVolume;
        set => Settings.MasterVolume = value;
    }

    public bool Fullscreen
    {
        get => Settings.Fullscreen;
        set => Settings.Fullscreen = value;
    }

    private GameSettings Settings
    {
        get
        {
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<GameSettings>();
                settings.ResetRuntimeToDefaults();
                Debug.LogWarning("[GameSettingsService] No hay GameSettings asset asignado. Se creo una configuracion runtime temporal.", this);
            }

            return settings;
        }
    }

    private void Awake()
    {
        if (loadOnAwake)
            Load();
    }

    public int GetMasterVolumeSegments()
    {
        return Settings.GetMasterVolumeSegments();
    }

    public void SetMasterVolumeFromSegments(float segments)
    {
        Settings.SetMasterVolumeFromSegments(segments);
    }

    public void Save()
    {
        var current = Settings;

        PlayerPrefs.SetFloat(current.BrightnessKey, current.Brightness);
        PlayerPrefs.SetFloat(current.MusicVolumeKey, current.MusicVolume);
        PlayerPrefs.SetFloat(current.EffectsVolumeKey, current.EffectsVolume);
        PlayerPrefs.SetFloat(current.MasterVolumeKey, current.MasterVolume);
        PlayerPrefs.SetInt(current.FullscreenKey, current.Fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        var current = Settings;

        current.Brightness = PlayerPrefs.GetFloat(current.BrightnessKey, current.DefaultBrightness);
        current.MusicVolume = PlayerPrefs.GetFloat(current.MusicVolumeKey, current.DefaultMusicVolume);
        current.EffectsVolume = PlayerPrefs.GetFloat(current.EffectsVolumeKey, current.DefaultEffectsVolume);
        current.MasterVolume = PlayerPrefs.GetFloat(current.MasterVolumeKey, current.DefaultMasterVolume);
        current.Fullscreen = PlayerPrefs.GetInt(current.FullscreenKey, current.DefaultFullscreen ? 1 : 0) == 1;
    }

    public void ResetToDefaults()
    {
        Settings.ResetRuntimeToDefaults();
        Save();
    }
}
