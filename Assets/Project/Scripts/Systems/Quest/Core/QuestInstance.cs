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

    public void AddProgress(ItemData_SO item, int amount, QuestTaskType type)
    {
        for (int i = 0; i < data.tasks.Count; i++)
        {
            var task = data.tasks[i];

            if (task.type != type) continue;
            if (task.targetItem != item) continue;

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