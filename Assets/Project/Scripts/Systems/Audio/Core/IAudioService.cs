/*
 * Arquitectura: Audio/Core
 * Script: IAudioService
 * Rol: Contrato minimo de reproduccion de audio para consumidores gameplay.
 * Relaciones: AudioService implementa este contrato; PlayerAudioFeedback y ScannerTrigger reciben el servicio por Inspector o jerarquia local.
 * Riesgo arquitectonico mitigado: evita que sistemas gameplay dependan directamente de un singleton concreto.
 */
public interface IAudioService
{
    void Play(AudioCueKey key);
    void Stop(AudioCueKey key);
    void ChangePitch(AudioCueKey key, float pitch);
}
