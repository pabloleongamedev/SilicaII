/*
 * Arquitectura: SaveLoad/Data
 * Script: ItemDatabase_SO
 * Rol: Database explicita para resolver items guardados por ID.
 * Relaciones: Implementa IItemResolver; PlayerInventorySaveSection lo usa para restaurar InventorySaveData.
 * Riesgo arquitectonico mitigado: elimina la necesidad de Resources.LoadAll y hace visibles las dependencias de data en Inspector.
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/SaveLoad/Item Database")]
public class ItemDatabase_SO : ScriptableObject, IItemResolver
{
    [SerializeField] private List<ItemData_SO> items = new List<ItemData_SO>();

    public bool TryResolveItem(string itemID, out ItemData_SO itemData)
    {
        itemData = null;

        if (string.IsNullOrEmpty(itemID))
            return false;

        foreach (var item in items)
        {
            if (item == null)
                continue;

            if (item.itemID == itemID)
            {
                itemData = item;
                return true;
            }
        }

        return false;
    }
}
