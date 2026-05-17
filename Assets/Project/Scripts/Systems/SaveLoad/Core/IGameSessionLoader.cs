/*
 * Arquitectura: SaveLoad/Core
 * Script: IGameSessionLoader
 * Rol: Contrato de flujo de partida usado por menu y botones.
 * Relaciones: MainMenuManager/SaveSlot lo consumen; GameManager lo implementa como fachada mientras se extrae la composicion de aplicacion.
 */
public interface IGameSessionLoader
{
    void LoadGame(string slotID);

    void CreateNewGame(string slotID);
}
