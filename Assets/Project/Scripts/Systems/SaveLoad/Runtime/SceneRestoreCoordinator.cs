/*
 * Arquitectura: SaveLoad/Runtime
 * Script: SceneRestoreCoordinator
 * Rol: Coordina captura/restauracion de objetos de escena usando SaveParticipantRegistry como ruta oficial.
 * Relaciones: GameManager delega aqui. SaveParticipantRegistry mantiene la lista explicita de participantes.
 * Riesgo arquitectonico mitigado: no usa busquedas globales; si faltan referencias, falla visible con warning.
 * Uso como referencia: SaveLoad no debe conocer Player, Inventory, HUD ni MissionTimer directamente.
 */
using UnityEngine;

public class SceneRestoreCoordinator
{
    private SaveParticipantRegistry participantRegistry;
    private ItemDatabase_SO itemDatabase;

    public SceneRestoreCoordinator(SaveParticipantRegistry participantRegistry, ItemDatabase_SO itemDatabase)
    {
        SetSceneDependencies(participantRegistry, itemDatabase);
    }

    public void SetSceneDependencies(SaveParticipantRegistry registry, ItemDatabase_SO database)
    {
        participantRegistry = registry;
        itemDatabase = database;
    }

    public void Capture(GameData gameData)
    {
        if (gameData == null)
            return;

        if (participantRegistry == null)
        {
            Debug.LogWarning("[SceneRestoreCoordinator] Falta SaveParticipantRegistry. No se capturo estado runtime.");
            return;
        }

        participantRegistry.Capture(gameData);
    }

    public void Restore(GameData gameData)
    {
        if (gameData == null)
            return;

        if (participantRegistry == null)
        {
            Debug.LogWarning("[SceneRestoreCoordinator] Falta SaveParticipantRegistry. No se restauro estado runtime.");
            return;
        }

        participantRegistry.Restore(gameData, itemDatabase);
    }
}
