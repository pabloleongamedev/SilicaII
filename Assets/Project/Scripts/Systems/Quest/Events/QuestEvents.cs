/*
 * Arquitectura: Quest/Events
 * Script: QuestEvents
 * Rol: Canal estatico temporal de Quest. Recibe eventos de progreso y publica snapshots hacia UI.
 * Modulo: Gestiona misiones y progreso a partir de eventos de gameplay como recolectar, refinar o craftear.
 * Relaciones: InventoryController/CraftingController/ChemistryController publican progreso; QuestSystem escucha; QuestUIController escucha carga y tareas.
 * Riesgo arquitectonico: el payload usa ItemData_SO y productores llaman QuestEvents directamente; debe migrar a adapters y payloads por itemID.
 * Uso como referencia: muestra Observer/Event-driven, pero tambien la deuda de ownership y payloads fuertemente acoplados.
 */
using System;

public static class QuestEvents
{
    // Sistema -> UI
    public static Action<QuestData_SO> OnQuestLoaded;
    public static Action<int, int, int, bool> OnTaskUpdated;
    public static Action<int> OnQuestCompleted;

    // Gameplay -> Sistema
    public static Action<ItemData_SO, int> OnItemCollected;
    public static Action<ItemData_SO, int> OnItemRefined;
    public static Action<ItemData_SO, int> OnItemCrafted;
}
