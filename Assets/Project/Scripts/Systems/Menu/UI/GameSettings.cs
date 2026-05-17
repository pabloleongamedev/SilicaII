/*
 * Arquitectura: Menu/UI
 * Script: GameSettings
 * Rol: Servicio de configuracion persistente basado en PlayerPrefs.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Implementa IGameSettingsReader/IGameSettingsWriter para que UI/audio puedan migrar fuera de GameSettings.Instance.
 * Uso como referencia: el singleton queda como acceso legacy; los consumidores nuevos deben depender de interfaces.
 */
using UnityEngine;

public class GameSettings : IGameSettingsReader, IGameSettingsWriter
{
    private static GameSettings instance;

    public static GameSettings Instance
    {
        get
        {
            if (instance == null)
                instance = new GameSettings();

            return instance;
        }
    }

    // Keys
    private const string BrightnessKey = "Brightness";
    private const string MusicVolumeKey = "MusicVolume";
    private const string EffectsVolumeKey = "EffectsVolume";
    private const string MasterVolumeKey = "MasterVolume";
    private const string FullscreenKey = "Fullscreen";

    // Valores actuales
    public float Brightness { get; set; }
    public float MusicVolume { get; set; }
    public float EffectsVolume { get; set; }

    // MasterVolume se guarda normalizado entre 0.1f y 1f
    public float MasterVolume { get; set; }
    public bool Fullscreen { get; set; }

    // Valores por defecto
    public const float DefaultBrightness = 0.75f;
    public const float DefaultMusic = 0.5f;
    public const float DefaultEffects = 0.5f;
    public const float DefaultMaster = 1f;
    public const bool DefaultFullscreen = true;

    // Rangos válidos
    public const float MinBrightness = 0f;
    public const float MaxBrightness = 1f;

    public const float MinVolume = 0f;
    public const float MaxVolume = 1f;

    public const float MinMasterVolume = 0.1f;
    public const float MaxMasterVolume = 1f;

    private GameSettings()
    {
        Load();
    }

    
    
    
    public void Save()
    {
        Brightness = Mathf.Clamp(Brightness, MinBrightness, MaxBrightness);
        MusicVolume = Mathf.Clamp(MusicVolume, MinVolume, MaxVolume);
        EffectsVolume = Mathf.Clamp(EffectsVolume, MinVolume, MaxVolume);
        MasterVolume = Mathf.Clamp(MasterVolume, MinMasterVolume, MaxMasterVolume);

        PlayerPrefs.SetFloat(BrightnessKey, Brightness);
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
        PlayerPrefs.SetFloat(EffectsVolumeKey, EffectsVolume);
        PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
        PlayerPrefs.SetInt(FullscreenKey, Fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        Brightness = Mathf.Clamp(
            PlayerPrefs.GetFloat(BrightnessKey, DefaultBrightness),
            MinBrightness,
            MaxBrightness
        );

        MusicVolume = Mathf.Clamp(
            PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusic),
            MinVolume,
            MaxVolume
        );

        EffectsVolume = Mathf.Clamp(
            PlayerPrefs.GetFloat(EffectsVolumeKey, DefaultEffects),
            MinVolume,
            MaxVolume
        );

        MasterVolume = Mathf.Clamp(
            PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMaster),
            MinMasterVolume,
            MaxMasterVolume
        );

        Fullscreen = PlayerPrefs.GetInt(FullscreenKey, DefaultFullscreen ? 1 : 0) == 1;
    }

    public void ResetToDefaults()
    {
        Brightness = DefaultBrightness;
        MusicVolume = DefaultMusic;
        EffectsVolume = DefaultEffects;
        MasterVolume = DefaultMaster;
        Fullscreen = DefaultFullscreen;
    }

    // Convierte el valor guardado (0.1f a 1f) al valor visual del slider (1 a 10)
    public int GetMasterVolumeSegments()
    {
        return Mathf.Clamp(Mathf.RoundToInt(MasterVolume * 10f), 1, 10);
    }

    // Convierte el valor del slider por segmentos (1 a 10) al valor normalizado (0.1f a 1f)
    public void SetMasterVolumeFromSegments(float segments)
    {
        segments = Mathf.Clamp(segments, 1f, 10f);
        MasterVolume = segments / 10f;
    }
}
