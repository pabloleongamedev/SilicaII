using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Quest/Quest")]
public class QuestData_SO : ScriptableObject
{
    public string questName;
    public List<QuestTask> tasks;
}

[System.Serializable]
public class QuestTask
{
    public string description;
    public QuestTaskType type;
    public ItemData_SO targetItem;
    public int requiredAmount;
}