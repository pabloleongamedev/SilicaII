/*
 * Arquitectura: Quest/Core
 * Script: IQuestEventListener
 * Rol: Contrato para sistemas que reaccionan a progreso de quest por itemID.
 * Relaciones: QuestSystem es el candidato natural para implementarlo en una fase posterior.
 */
public interface IQuestEventListener
{
    void HandleItemCollected(string itemID, int amount);

    void HandleItemRefined(string itemID, int amount);

    void HandleItemCrafted(string itemID, int amount);
}
