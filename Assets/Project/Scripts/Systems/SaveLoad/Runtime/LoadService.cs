/*
 * Arquitectura: SaveLoad/Runtime
 * Script: LoadService
 * Rol: Servicio de carga desde persistencia. No restaura escena; solo entrega GameData.
 * Relaciones: GameManager lo usa antes de decidir si recarga escena o coordina restauracion runtime.
 */
public class LoadService
{
    private readonly SaveController saveController;

    public LoadService(SaveController saveController)
    {
        this.saveController = saveController;
    }

    public GameData LoadGame(string slotID)
    {
        return saveController.LoadGame(slotID);
    }
}
