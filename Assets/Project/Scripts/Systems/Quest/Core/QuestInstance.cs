/*
 * Arquitectura: Quest/Core
 * Script: QuestInstance
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona misiones y progreso a partir de eventos de gameplay como recolectar, refinar o craftear.
 * Relaciones: Aplica progreso por itemID; QuestSystem decide cuando publicar eventos hacia UI.
 * Fase 5: Core ya no necesita comparar instancias ItemData_SO entre sistemas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System.Collections.Generic;

public class QuestInstance
{
    public QuestData_SO data;

    private Dictionary<int, int> progress = new();

    public QuestInstance(QuestData_SO data)
    {
        this.data = data;

        for (int i = 0; i < data.tasks.Count; i++)
            progress[i] = 0;
    }

    public void AddProgress(string itemID, int amount, QuestTaskType type)
    {
        if (string.IsNullOrEmpty(itemID))
            return;

        for (int i = 0; i < data.tasks.Count; i++)
        {
            var task = data.tasks[i];

            if (task.type != type) continue;
            if (task.targetItem == null || task.targetItem.itemID != itemID) continue;

            progress[i] += amount;

            int current = progress[i];
            int required = task.requiredAmount;

            bool completed = current >= required;

            QuestEvents.OnTaskUpdated?.Invoke(i, current, required, completed);
        }
    }
    

    public bool IsComplete()
    {
        for (int i = 0; i < data.tasks.Count; i++)
        {
            if (progress[i] < data.tasks[i].requiredAmount)
                return false;
        }
        return true;
    }
}
