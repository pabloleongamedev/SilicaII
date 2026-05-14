/*
 * Arquitectura: SaveLoad/Core
 * Script: IItemResolver
 * Rol: Contrato de resolucion de items persistidos. SaveLoad recibe itemID y no decide donde viven los ScriptableObjects.
 * Relaciones: GameRestorer e InventorySaveParticipant lo usan para convertir InventorySaveData.itemID en ItemData_SO.
 * Riesgo arquitectonico mitigado: reemplaza Resources.LoadAll como dependencia implicita por una frontera explicita.
 */
public interface IItemResolver
{
    bool TryResolveItem(string itemID, out ItemData_SO itemData);
}
