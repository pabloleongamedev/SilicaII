/*
 * Arquitectura: Menu/Data
 * Script: GameSettings
 * Rol: Asset de configuracion de opciones de usuario.
 * Relaciones: GameSettingsService usa este asset como fuente editable y persistente; UI/audio consumen solo IGameSettingsReader/Writer.
 * Riesgo arquitectonico mitigado: elimina el acceso global y deja la configuracion visible por Inspector.
 * Uso como referencia: el asset define valores/rangos/defaults; el service decide cuando cargar/guardar en PlayerPrefs.
 */
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Menu/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("PlayerPrefs Keys")]
    [SerializeField] private string brightnessKey = "Brightness";
    [SerializeField] private string musicVolumeKey = "MusicVolume";
    [SerializeField] private string effectsVolumeKey = "EffectsVolume";
    [SerializeField] private string masterVolumeKey = "MasterVolume";
    [SerializeField] private string fullscreenKey = "Fullscreen";

    [Header("Defaults")]
    [SerializeField, Range(0f, 1f)] private float defaultBrightness = 0.75f;
    [SerializeField, Range(0f, 1f)] private float defaultMusicVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float defaultEffectsVolume = 0.5f;
    [SerializeField, Range(0.1f, 1f)] private float defaultMasterVolume = 1f;
    [SerializeField] private bool defaultFullscreen = true;

    [Header("Runtime Values")]
    [SerializeField, Range(0f, 1f)] private float brightness = 0.75f;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = 0.5f;
    [SerializeField, Range(0.1f, 1f)] private float masterVolume = 1f;
    [SerializeField] private bool fullscreen = true;

    public string BrightnessKey => brightnessKey;
    public string MusicVolumeKey => musicVolumeKey;
    public string EffectsVolumeKey => effectsVolumeKey;
    public string MasterVolumeKey => masterVolumeKey;
    public string FullscreenKey => fullscreenKey;

    public float DefaultBrightness => defaultBrightness;
    public float DefaultMusicVolume => defaultMusicVolume;
    public float DefaultEffectsVolume => defaultEffectsVolume;
    public float DefaultMasterVolume => defaultMasterVolume;
    public bool DefaultFullscreen => defaultFullscreen;

    public float Brightness
    {
        get => brightness;
        set => brightness = Mathf.Clamp01(value);
    }

    public float MusicVolume
    {
        get => musicVolume;
        set => musicVolume = Mathf.Clamp01(value);
    }

    public float EffectsVolume
    {
        get => effectsVolume;
        set => effectsVolume = Mathf.Clamp01(value);
    }

    public float MasterVolume
    {
        get => masterVolume;
        set => masterVolume = Mathf.Clamp(value, 0.1f, 1f);
    }

    public bool Fullscreen
    {
        get => fullscreen;
        set => fullscreen = value;
    }

    public int GetMasterVolumeSegments()
    {
        return Mathf.Clamp(Mathf.RoundToInt(MasterVolume * 10f), 1, 10);
    }

    public void SetMasterVolumeFromSegments(float segments)
    {
        MasterVolume = Mathf.Clamp(segments, 1f, 10f) / 10f;
    }

    public void ResetRuntimeToDefaults()
    {
        Brightness = defaultBrightness;
        MusicVolume = defaultMusicVolume;
        EffectsVolume = defaultEffectsVolume;
        MasterVolume = defaultMasterVolume;
        Fullscreen = defaultFullscreen;
    }
}
