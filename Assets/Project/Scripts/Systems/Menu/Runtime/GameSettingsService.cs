/*
 * Arquitectura: Menu/Runtime
 * Script: GameSettingsService
 * Rol: Adapter de escena para configuracion persistente.
 * Relaciones: Implementa IGameSettingsReader/IGameSettingsWriter y envuelve GameSettings, que conserva PlayerPrefs como detalle interno.
 * Riesgo arquitectonico mitigado: UI y Audio dependen de contratos asignables por Inspector, no de GameSettings.Instance.
 * Uso como referencia: si luego cambias PlayerPrefs por archivo, nube o perfil de usuario, los consumidores no cambian.
 */
using UnityEngine;

public class GameSettingsService : MonoBehaviour, IGameSettingsReader, IGameSettingsWriter
{
    private GameSettings Settings => GameSettings.Instance;

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
        Settings.Save();
    }

    public void Load()
    {
        Settings.Load();
    }

    public void ResetToDefaults()
    {
        Settings.ResetToDefaults();
    }
}
