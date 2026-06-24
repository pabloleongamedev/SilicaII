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
    private const float FallbackBrightness = 0.75f;
    private const float FallbackMusicVolume = 0.5f;
    private const float FallbackEffectsVolume = 0.5f;
    private const float FallbackMasterVolume = 1f;
    private const bool FallbackFullscreen = true;

    [SerializeField] private GameSettings settings;
    [SerializeField] private bool loadOnAwake = true;

    private bool warnedMissingSettings;

    public float Brightness
    {
        get => HasSettings ? settings.Brightness : FallbackBrightness;
        set
        {
            if (HasSettings)
                settings.Brightness = value;
        }
    }

    public float MusicVolume
    {
        get => HasSettings ? settings.MusicVolume : FallbackMusicVolume;
        set
        {
            if (HasSettings)
                settings.MusicVolume = value;
        }
    }

    public float EffectsVolume
    {
        get => HasSettings ? settings.EffectsVolume : FallbackEffectsVolume;
        set
        {
            if (HasSettings)
                settings.EffectsVolume = value;
        }
    }

    public float MasterVolume
    {
        get => HasSettings ? settings.MasterVolume : FallbackMasterVolume;
        set
        {
            if (HasSettings)
                settings.MasterVolume = value;
        }
    }

    public bool Fullscreen
    {
        get => HasSettings ? settings.Fullscreen : FallbackFullscreen;
        set
        {
            if (HasSettings)
                settings.Fullscreen = value;
        }
    }

    private bool HasSettings
    {
        get
        {
            if (settings != null)
                return true;

            if (!warnedMissingSettings)
            {
                warnedMissingSettings = true;
                Debug.LogWarning("[GameSettingsService] Asigna un GameSettings asset por Inspector. Se usaran defaults runtime y no se persistiran cambios.", this);
            }

            return false;
        }
    }

    private void Awake()
    {
        if (loadOnAwake)
            Load();
    }

    public int GetMasterVolumeSegments()
    {
        return HasSettings ? settings.GetMasterVolumeSegments() : Mathf.Clamp(Mathf.RoundToInt(FallbackMasterVolume * 10f), 1, 10);
    }

    public void SetMasterVolumeFromSegments(float segments)
    {
        if (HasSettings)
            settings.SetMasterVolumeFromSegments(segments);
    }

    public void Save()
    {
        if (!HasSettings)
            return;

        var current = settings;

        PlayerPrefs.SetFloat(current.BrightnessKey, current.Brightness);
        PlayerPrefs.SetFloat(current.MusicVolumeKey, current.MusicVolume);
        PlayerPrefs.SetFloat(current.EffectsVolumeKey, current.EffectsVolume);
        PlayerPrefs.SetFloat(current.MasterVolumeKey, current.MasterVolume);
        PlayerPrefs.SetInt(current.FullscreenKey, current.Fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (!HasSettings)
            return;

        var current = settings;

        current.Brightness = PlayerPrefs.GetFloat(current.BrightnessKey, current.DefaultBrightness);
        current.MusicVolume = PlayerPrefs.GetFloat(current.MusicVolumeKey, current.DefaultMusicVolume);
        current.EffectsVolume = PlayerPrefs.GetFloat(current.EffectsVolumeKey, current.DefaultEffectsVolume);
        current.MasterVolume = PlayerPrefs.GetFloat(current.MasterVolumeKey, current.DefaultMasterVolume);
        current.Fullscreen = PlayerPrefs.GetInt(current.FullscreenKey, current.DefaultFullscreen ? 1 : 0) == 1;
    }

    public void ResetToDefaults()
    {
        if (!HasSettings)
            return;

        settings.ResetRuntimeToDefaults();
        Save();
    }
}
