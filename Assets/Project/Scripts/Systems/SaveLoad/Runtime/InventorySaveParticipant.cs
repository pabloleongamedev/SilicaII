/*
 * Arquitectura: SaveLoad/Runtime
 * Script: InventorySaveParticipant
 * Rol: Participante de guardado para Inventory.
 * Relaciones: Implementa ISaveParticipant; usa InventoryController como facade y IItemResolver para restaurar ItemData_SO desde itemID.
 * Riesgo arquitectonico mitigado: SaveLoad deja de conocer detalles internos de Inventory y delega export/import al sistema dueno.
 */
using UnityEngine;

public class InventorySaveParticipant : MonoBehaviour, ISaveParticipant
{
    [SerializeField] private InventoryController inventory;

    public void Capture(GameData gameData)
    {
        if (gameData == null || inventory == null)
            return;

        gameData.inventoryItems = inventory.ExportSaveData();
    }

    public void Restore(GameData gameData, IItemResolver itemResolver)
    {
        if (gameData == null || inventory == null)
            return;

        inventory.ImportSaveData(gameData.inventoryItems, ResolveItem);

        ItemData_SO ResolveItem(string itemID)
        {
            if (itemResolver == null)
                return null;

            return itemResolver.TryResolveItem(itemID, out var itemData)
                ? itemData
                : null;
        }
    }
}
