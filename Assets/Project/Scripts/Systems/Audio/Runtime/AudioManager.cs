/*
 * Arquitectura: Audio/Runtime
 * Script: AudioManager
 * Rol: Adapter de compatibilidad para escenas antiguas.
 * Relaciones: Hereda AudioService para que referencias serializadas viejas sigan reproduciendo audio sin exponer AudioManager.Instance.
 * Riesgo arquitectonico mitigado: el singleton fue eliminado; nuevos objetos deben agregar AudioService directamente.
 */
public class AudioManager : AudioService
{
}
