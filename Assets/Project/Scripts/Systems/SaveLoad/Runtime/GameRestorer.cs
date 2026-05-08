/*
 * Arquitectura: SaveLoad/Runtime
 * Script: GameRestorer
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona datos de partida, guardado/carga y restauracion de estado runtime.
 * Relaciones: Consulta facades runtime como InventoryController y PlayerInputHandler para persistir/restaurar datos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System.Collections;
using UnityEngine;

public class GameRestorer : MonoBehaviour
{
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

        var items = Resources.LoadAll<ItemData_SO>(string.Empty);
        foreach (var item in items)
        {
            if (item.itemID == itemID)
                return item;
        }

        Debug.LogWarning($"[GameRestorer] ItemData no encontrado para itemID: {itemID}");
        return null;
    }
}
