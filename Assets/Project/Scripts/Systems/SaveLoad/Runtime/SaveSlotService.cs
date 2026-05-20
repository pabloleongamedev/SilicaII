/*
 * Arquitectura: SaveLoad/Runtime
 * Script: SaveSlotService
 * Rol: Servicio de lectura de slots. Encapsula SaveController para que Menu/UI no conozca disco ni JSON.
 * Relaciones: SaveLoadSceneBinding delega consultas de slots aqui; SaveSlot y MainMenuManager consumen ISaveSlotReader.
 */
public class SaveSlotService : ISaveSlotReader
{
    private readonly SaveController saveController;

    public SaveSlotService(SaveController saveController)
    {
        this.saveController = saveController;
    }

    public bool HasSaveFile(string slotID)
    {
        return saveController.HasSaveFile(slotID);
    }

    public SaveInfo GetSaveInfo(string slotID)
    {
        return saveController.GetSaveInfo(slotID);
    }

    public SaveInfo[] GetAllSaveInfos()
    {
        return saveController.GetAllSaveInfos();
    }
}
