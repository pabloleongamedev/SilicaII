/*
 * Arquitectura: Quest/Events
 * Script: QuestEvents
 * Rol: Canal estatico temporal de Quest. Recibe eventos de progreso y publica snapshots hacia UI.
 * Modulo: Gestiona misiones y progreso a partir de eventos de gameplay como recolectar, refinar o craftear.
 * Relaciones: GameplayEventRouter traduce eventos de Inventory/Crafting a itemID; QuestSystem escucha progreso por ID; UI escucha snapshots.
 * Fase 5: el payload de progreso ya no expone ItemData_SO, reduciendo acoplamiento de Quest con assets de Inventory.
 * Siguiente migracion: reemplazar llamadas estaticas por IQuestEventPublisher/IQuestEventListener inyectados por escena o composition root.
 */
using System;

public static class QuestEvents
{
    // Sistema -> UI
    public static Action<QuestData_SO> OnQuestLoaded;
    public static Action<int, int, int, bool> OnTaskUpdated;
    public static Action<int> OnQuestCompleted;

    // Gameplay -> Sistema
    public static Action<string, int> OnItemCollected;
    public static Action<string, int> OnItemRefined;
    public static Action<string, int> OnItemCrafted;
    private static QuestEventChannel_SO channel;

    public static void ConfigureChannel(QuestEventChannel_SO eventChannel)
    {
        channel = eventChannel;
    }

    public static void PublishQuestLoaded(QuestData_SO questData)
    {
        OnQuestLoaded?.Invoke(questData);
        channel?.RaiseQuestLoaded(questData);
    }

    public static void PublishTaskUpdated(int taskIndex, int current, int required, bool completed)
    {
        OnTaskUpdated?.Invoke(taskIndex, current, required, completed);
        channel?.RaiseTaskUpdated(taskIndex, current, required, completed);
    }

    public static void PublishQuestCompleted(int questIndex)
    {
        OnQuestCompleted?.Invoke(questIndex);
        channel?.RaiseQuestCompleted(questIndex);
    }

    public static void PublishItemCollected(string itemID, int amount)
    {
        OnItemCollected?.Invoke(itemID, amount);
        channel?.RaiseItemCollected(itemID, amount);
    }

    public static void PublishItemRefined(string itemID, int amount)
    {
        OnItemRefined?.Invoke(itemID, amount);
        channel?.RaiseItemRefined(itemID, amount);
    }

    public static void PublishItemCrafted(string itemID, int amount)
    {
        OnItemCrafted?.Invoke(itemID, amount);
        channel?.RaiseItemCrafted(itemID, amount);
    }
}
