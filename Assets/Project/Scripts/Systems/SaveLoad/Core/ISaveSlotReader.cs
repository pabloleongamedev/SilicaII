/*
 * Arquitectura: SaveLoad/Core
 * Script: ISaveSlotReader
 * Rol: Contrato de lectura para UI de slots; no expone guardado, escena ni estado runtime.
 * Relaciones: SaveSlot/MainMenuManager lo consumen para mostrar Continuar/Nueva Partida sin conocer SaveController.
 */
public interface ISaveSlotReader
{
    bool HasSaveFile(string slotID);

    SaveInfo GetSaveInfo(string slotID);

    SaveInfo[] GetAllSaveInfos();
}
