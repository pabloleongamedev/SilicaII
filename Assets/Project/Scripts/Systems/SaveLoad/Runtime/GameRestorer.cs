/*
 * Arquitectura: SaveLoad/Runtime
 * Script: GameRestorer
 * Rol: Adapter runtime de restauracion. Toma GameData activo y lo entrega a SceneRestoreCoordinator.
 * Modulo: Coordina restauracion diferida cuando una escena termina de inicializarse.
 * Relaciones: Consume IGameDataProvider por Inspector; SaveParticipantRegistry es la ruta oficial de restauracion.
 * Riesgo arquitectonico mitigado: ya no duplica restauracion manual; comparte la misma ruta que GameManager.
 * Nota: restoreOnStart debe activarse solo en escenas que no sean restauradas por GameManager.OnSceneLoaded.
 * Uso como referencia: muestra como un MonoBehaviour de escena consume contratos y delega en servicios.
 */
using System.Collections;
using UnityEngine;

public class GameRestorer : MonoBehaviour
{
    [Header("Decoupled Restore")]
    [SerializeField] private bool restoreOnStart = false;
    [SerializeField] private MonoBehaviour gameDataProviderBehaviour;
    [SerializeField] private SaveParticipantRegistry participantRegistry;
    [SerializeField] private ItemDatabase_SO itemDatabase;

    private IGameDataProvider gameDataProvider;
    private SceneRestoreCoordinator restoreCoordinator;

    private void Awake()
    {
        gameDataProvider = gameDataProviderBehaviour as IGameDataProvider;
        restoreCoordinator = new SceneRestoreCoordinator(participantRegistry, itemDatabase);
    }

    private void Start()
    {
        if (restoreOnStart)
            StartCoroutine(RestoreDelayed());
    }

    public void RestoreCurrentGameData()
    {
        StartCoroutine(RestoreDelayed());
    }

    private IEnumerator RestoreDelayed()
    {
        yield return null;

        var gameData = gameDataProvider?.GetCurrentGameData();
        if (gameData == null)
        {
            Debug.LogWarning("[GameRestorer] No existe IGameDataProvider asignado o no hay GameData activo.", this);
            yield break;
        }

        restoreCoordinator.SetSceneDependencies(participantRegistry, itemDatabase);
        restoreCoordinator.Restore(gameData);
    }
}
