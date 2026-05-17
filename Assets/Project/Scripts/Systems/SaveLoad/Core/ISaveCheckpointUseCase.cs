/*
 * Arquitectura: SaveLoad/Core
 * Script: ISaveCheckpointUseCase
 * Rol: Contrato de caso de uso para guardar checkpoints sin depender de GameManager concreto.
 * Relaciones: CheckpointSavePoint consume esta interfaz; GameManager la implementa como fachada temporal.
 * Uso como referencia: la UI/interaccion habla con intenciones de guardado, no con singletons ni detalles de disco.
 */
public interface ISaveCheckpointUseCase
{
    string CurrentSlotID { get; }

    bool TrySaveCheckpoint(bool createIfMissing);
}
