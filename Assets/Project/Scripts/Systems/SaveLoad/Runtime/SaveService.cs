/*
 * Arquitectura: SaveLoad/Runtime
 * Script: SaveService
 * Rol: Servicio de escritura de partidas. Recibe GameData ya capturado y lo persiste mediante SaveController.
 * Relaciones: GameManager delega aqui SaveGame/Checkpoint/Autosave; SceneRestoreCoordinator captura estado antes de llamar este servicio.
 */
public class SaveService
{
    private readonly SaveController saveController;

    public SaveService(SaveController saveController)
    {
        this.saveController = saveController;
    }

    public bool SaveGame(GameData gameData, string slotID)
    {
        return saveController.SaveGame(gameData, slotID);
    }
}
