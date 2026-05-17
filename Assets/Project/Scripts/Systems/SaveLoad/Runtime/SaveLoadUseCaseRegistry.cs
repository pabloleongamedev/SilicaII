/*
 * Arquitectura: SaveLoad/Runtime
 * Script: SaveLoadUseCaseRegistry
 * Rol: Registry runtime pequeno para publicar casos de uso activos de SaveLoad sin acoplar interactuables a GameManager.
 * Relaciones: GameManager registra sus interfaces; CheckpointSavePoint/CheckpointRestorePoint consultan contratos, no clases concretas.
 * Uso como referencia: puente de composicion para escenas donde los cubos no tienen el adapter asignado en Inspector.
 */
using UnityEngine;

public static class SaveLoadUseCaseRegistry
{
    public static ISaveCheckpointUseCase SaveCheckpointUseCase { get; private set; }
    public static IRestoreCheckpointUseCase RestoreCheckpointUseCase { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        SaveCheckpointUseCase = null;
        RestoreCheckpointUseCase = null;
    }

    public static void Register(ISaveCheckpointUseCase saveUseCase, IRestoreCheckpointUseCase restoreUseCase)
    {
        SaveCheckpointUseCase = saveUseCase;
        RestoreCheckpointUseCase = restoreUseCase;
    }

    public static void Unregister(ISaveCheckpointUseCase saveUseCase, IRestoreCheckpointUseCase restoreUseCase)
    {
        if (ReferenceEquals(SaveCheckpointUseCase, saveUseCase))
            SaveCheckpointUseCase = null;

        if (ReferenceEquals(RestoreCheckpointUseCase, restoreUseCase))
            RestoreCheckpointUseCase = null;
    }
}
