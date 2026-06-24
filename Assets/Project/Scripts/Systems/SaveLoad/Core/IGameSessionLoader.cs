/*
 * Arquitectura: SaveLoad/Core
 * Script: IGameSessionLoader
 * Rol: Contrato de flujo de partida usado por menu y botones.
 * Relaciones: MainMenuManager/SaveSlot lo consumen; SaveLoadSceneBinding lo implementa como fachada de escena.
 */
public interface IGameSessionLoader
{
    void LoadGame(string slotID);

    void CreateNewGame(string slotID);
}
