/*
 * Arquitectura: SaveLoad/Core
 * Script: IRestoreCheckpointUseCase
 * Rol: Contrato de caso de uso para restaurar checkpoints sin depender de una implementacion concreta de SaveLoad.
 * Relaciones: CheckpointRestorePoint consume esta interfaz; SaveLoadSceneBinding la implementa como fachada de escena.
 * Uso como referencia: separa el objeto interactuable de la decision de cargar escena o restaurar estado runtime.
 */
public interface IRestoreCheckpointUseCase
{
    string CurrentSlotID { get; }

    bool HasCheckpoint(string slotID);

    void RestoreCheckpoint(string slotID, bool reloadScene);
}
