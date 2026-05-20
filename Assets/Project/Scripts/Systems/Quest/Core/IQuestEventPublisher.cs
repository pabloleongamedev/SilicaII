/*
 * Arquitectura: Quest/Core
 * Script: IQuestEventPublisher
 * Rol: Contrato para publicar progreso de quest sin depender de canales globales.
 * Relaciones: QuestEventChannel_SO es la salida oficial de escena; routers o tests pueden adaptar este contrato.
 */
public interface IQuestEventPublisher
{
    void PublishItemCollected(string itemID, int amount);

    void PublishItemRefined(string itemID, int amount);

    void PublishItemCrafted(string itemID, int amount);
}
