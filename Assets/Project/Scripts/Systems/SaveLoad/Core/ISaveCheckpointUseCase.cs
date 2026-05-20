/*
 * Arquitectura: SaveLoad/Core
 * Script: ISaveCheckpointUseCase
 * Rol: Contrato de caso de uso para guardar checkpoints sin depender de una implementacion concreta de SaveLoad.
 * Relaciones: CheckpointSavePoint consume esta interfaz; SaveLoadSceneBinding la implementa como fachada de escena.
 * Uso como referencia: la UI/interaccion habla con intenciones de guardado, no con singletons ni detalles de disco.
 */
public interface ISaveCheckpointUseCase
{
    string CurrentSlotID { get; }

    bool TrySaveCheckpoint(bool createIfMissing);
}
