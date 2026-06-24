/*
 * Arquitectura: Quest/Core
 * Script: QuestTaskInstance
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona misiones y progreso a partir de eventos de gameplay como recolectar, refinar o craftear.
 * Relaciones: Progresa por itemID para mantener Quest desacoplado de referencias concretas de Inventory.
 * Fase 5: evita comparar ItemData_SO por referencia en reglas de progreso.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class QuestTaskInstance
{
    private QuestTask data;

    public int CurrentAmount { get; private set; }

    public bool IsComplete => CurrentAmount >= data.requiredAmount;

    public QuestTaskInstance(QuestTask data)
    {
        this.data = data;
        CurrentAmount = 0;
    }

    public bool Progress(string itemID, int amount, QuestTaskType type)
    {
        if (data == null)
            return false;

        if (data.targetItem == null)
            return false;

        if (data.type != type)
            return false;

        if (data.targetItem.itemID != itemID)
            return false;

        CurrentAmount += amount;

        if (CurrentAmount > data.requiredAmount)
            CurrentAmount = data.requiredAmount;

        return true; // indica que esta tarea fue afectada
    }

    public int GetRequiredAmount()
    {
        return data.requiredAmount;
    }

    public string GetDescription()
    {
        return data.description;
    }
}
