/*
 * Arquitectura: Menu/Core
 * Script: IGameSettingsReader
 * Rol: Contrato de lectura de configuracion para UI/audio sin depender de una configuracion global.
 */
public interface IGameSettingsReader
{
    float Brightness { get; }
    float MusicVolume { get; }
    float EffectsVolume { get; }
    float MasterVolume { get; }
    bool Fullscreen { get; }

    int GetMasterVolumeSegments();
}
