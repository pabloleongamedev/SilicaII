/*
 * Arquitectura: Quest/Events
 * Script: QuestEvents
 * Rol: Canal de comunicacion desacoplado entre sistemas. Permite Observer/Event-driven sin referencias profundas.
 * Modulo: Gestiona misiones y progreso a partir de eventos de gameplay como recolectar, refinar o craftear.
 * Relaciones: Escucha eventos de Inventory/Crafting y publica estado de mision hacia UI u otros sistemas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
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
