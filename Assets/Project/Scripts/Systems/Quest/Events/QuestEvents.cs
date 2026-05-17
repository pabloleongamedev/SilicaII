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
}
