/*
 * Arquitectura: Audio/Core
 * Script: IAudioService
 * Rol: Contrato minimo de reproduccion de audio para consumidores gameplay.
 * Relaciones: AudioService implementa este contrato; PlayerAudio y ScannerTrigger reciben el servicio por Inspector o jerarquia local.
 * Riesgo arquitectonico mitigado: evita que sistemas gameplay dependan directamente de un singleton concreto.
 */
public interface IAudioService
{
    void Play(AudioCue_SO cue);
    void Stop(AudioCue_SO cue);
    void Play(string name);
    void Stop(string name);
    void ChangePitch(string name, float pitch);
}
