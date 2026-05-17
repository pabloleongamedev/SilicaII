/*
 * Arquitectura: SaveLoad/Core
 * Script: IGameDataProvider
 * Rol: Contrato minimo para leer el GameData activo sin acoplar restauradores al GameManager.
 * Relaciones: GameRestorer lo consume cuando una escena necesita restaurar despues de cargar.
 */
public interface IGameDataProvider
{
    GameData GetCurrentGameData();
}
