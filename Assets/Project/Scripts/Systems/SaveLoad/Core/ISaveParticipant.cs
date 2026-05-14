/*
 * Arquitectura: SaveLoad/Core
 * Script: ISaveParticipant
 * Rol: Contrato para que cada sistema exporte/restaure su propia parte de GameData.
 * Relaciones: SaveLoad orquesta participantes; Player/Inventory implementan participantes concretos sin que GameManager conozca sus detalles internos.
 * Riesgo arquitectonico mitigado: evita que GameManager siga creciendo con metodos UpdateMySystem o FindFirstObjectByType por cada nuevo sistema.
 */
public interface ISaveParticipant
{
    void Capture(GameData gameData);
    void Restore(GameData gameData, IItemResolver itemResolver);
}
