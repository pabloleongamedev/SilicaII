/*
 * Arquitectura: Audio/Core
 * Script: IAudioService
 * Rol: Contrato minimo de reproduccion de audio para consumidores gameplay.
 * Relaciones: AudioManager implementa este contrato; PlayerAudio y ScannerTrigger pueden migrar desde el singleton hacia una referencia de servicio.
 * Riesgo arquitectonico mitigado: evita que sistemas gameplay dependan directamente del singleton concreto de AudioManager.
 */
public interface IAudioService
{
    void Play(string name);
    void Stop(string name);
    void ChangePitch(string name, float pitch);
}
