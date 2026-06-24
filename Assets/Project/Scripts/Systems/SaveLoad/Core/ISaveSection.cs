/*
 * Arquitectura: SaveLoad/Core
 * Script: ISaveSection
 * Rol: Contrato pequeno para una seccion pura de guardado/restauracion dentro de un participante mayor.
 * Relaciones: PlayerSaveParticipant compone varias secciones para evitar una mega clase sin multiplicar MonoBehaviours en Inspector.
 * Uso como referencia: los participantes son adapters Unity; las secciones concentran piezas concretas de persistencia.
 */
public interface ISaveSection
{
    void Capture(GameData gameData);

    void Restore(GameData gameData, IItemResolver itemResolver);
}
