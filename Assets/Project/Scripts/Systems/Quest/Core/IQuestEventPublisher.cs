/*
 * Arquitectura: Quest/Core
 * Script: IQuestEventPublisher
 * Rol: Contrato para publicar progreso de quest sin depender del bus estatico QuestEvents.
 * Relaciones: QuestEvents puede actuar como implementacion legacy; routers futuros pueden inyectar este contrato.
 */
public interface IQuestEventPublisher
{
    void PublishItemCollected(string itemID, int amount);

    void PublishItemRefined(string itemID, int amount);

    void PublishItemCrafted(string itemID, int amount);
}
