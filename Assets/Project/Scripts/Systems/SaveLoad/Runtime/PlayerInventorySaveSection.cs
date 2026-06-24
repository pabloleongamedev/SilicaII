/*
 * Arquitectura: SaveLoad/Runtime
 * Script: PlayerInventorySaveSection
 * Rol: Seccion no-MonoBehaviour para persistir inventario del Player.
 * Relaciones: Usada por PlayerSaveParticipant; InventoryController conserva ownership de export/import.
 */
using UnityEngine;

public class PlayerInventorySaveSection : ISaveSection
{
    private readonly InventoryController inventory;
    private readonly Object logContext;

    public PlayerInventorySaveSection(InventoryController inventory, Object logContext)
    {
        this.inventory = inventory;
        this.logContext = logContext;
    }

    public void Capture(GameData gameData)
    {
        if (gameData == null || inventory == null)
        {
            WarnMissingInventory();
            return;
        }

        gameData.inventoryItems = inventory.ExportSaveData();
    }

    public void Restore(GameData gameData, IItemResolver itemResolver)
    {
        if (gameData == null || inventory == null)
        {
            WarnMissingInventory();
            return;
        }

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

    private void WarnMissingInventory()
    {
        if (inventory == null)
            Debug.LogWarning("[PlayerInventorySaveSection] Falta InventoryController en PlayerSaveParticipant.", logContext);
    }
}
