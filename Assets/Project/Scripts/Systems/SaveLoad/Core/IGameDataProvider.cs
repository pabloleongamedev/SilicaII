/*
 * Arquitectura: SaveLoad/Core
 * Script: IGameDataProvider
 * Rol: Contrato minimo para leer el GameData activo sin acoplar restauradores al GameManager.
 * Relaciones: adaptadores de escena pueden consumirlo cuando necesitan leer la partida activa.
 */
public interface IGameDataProvider
{
    GameData GetCurrentGameData();
}
