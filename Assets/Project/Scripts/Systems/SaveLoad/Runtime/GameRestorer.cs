/*
 * Arquitectura: SaveLoad/Runtime
 * Script: GameRestorer
 * Rol: Adapter runtime de restauracion. Toma GameData activo y lo aplica a objetos de escena.
 * Modulo: Gestiona restauracion de posicion/rotacion del jugador e inventario guardado.
 * Relaciones: Lee GameManager.Instance y puede restaurar mediante SaveParticipantRegistry + ItemDatabase_SO.
 * Riesgo arquitectonico: conserva fallback legacy con FindFirstObjectByType mientras las escenas se migran a participantes explicitos.
 * Uso como referencia: muestra el puente entre flujo antiguo y SaveParticipants, priorizando dependencias visibles por Inspector.
 */
using System.Collections;
using UnityEngine;

public class GameRestorer : MonoBehaviour
{
    [Header("Decoupled Restore")]
    [SerializeField] private SaveParticipantRegistry participantRegistry;
    [SerializeField] private ItemDatabase_SO itemDatabase;

    private void Start()
    {
        StartCoroutine(RestoreDelayed());
    }

    private IEnumerator RestoreDelayed()
    {
        yield return null;

        var gameData = GameManager.Instance?.GetCurrentGameData();
        if (gameData == null)
            yield break;

        if (participantRegistry != null)
        {
            participantRegistry.Restore(gameData, itemDatabase);
            yield break;
        }

        Debug.LogWarning("[GameRestorer] SaveParticipantRegistry no asignado. Usando restauracion legacy temporal.");

        var playerInput = FindFirstObjectByType<PlayerInputHandler>();
        if (playerInput != null)
        {
            playerInput.transform.SetPositionAndRotation(
                gameData.playerData.GetPosition(),
                gameData.playerData.GetRotation()
            );
        }

        var inventory = FindFirstObjectByType<InventoryController>();
        if (inventory != null)
            inventory.ImportSaveData(gameData.inventoryItems, ResolveItem);
    }

    private ItemData_SO ResolveItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return null;

        if (itemDatabase != null && itemDatabase.TryResolveItem(itemID, out var itemData))
            return itemData;

        Debug.LogWarning($"[GameRestorer] ItemData no encontrado para itemID: {itemID}. Asigna ItemDatabase_SO para evitar Resources.LoadAll.");
        return null;
    }
}
