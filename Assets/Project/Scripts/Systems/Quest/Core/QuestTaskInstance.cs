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

    public bool Progress(ItemData_SO item, int amount, QuestTaskType type)
    {
        if (data == null)
            return false;

        if (data.targetItem == null)
            return false;

        if (data.type != type)
            return false;

        if (data.targetItem != item)
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