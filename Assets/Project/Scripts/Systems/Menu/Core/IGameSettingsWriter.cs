/*
 * Arquitectura: Menu/Core
 * Script: IGameSettingsWriter
 * Rol: Contrato de escritura de configuracion. Permite reemplazar PlayerPrefs o GameSettings sin tocar UI.
 */
public interface IGameSettingsWriter
{
    float Brightness { set; }
    float MusicVolume { set; }
    float EffectsVolume { set; }
    float MasterVolume { set; }
    bool Fullscreen { set; }

    void SetMasterVolumeFromSegments(float segments);
    void Save();
    void Load();
    void ResetToDefaults();
}
