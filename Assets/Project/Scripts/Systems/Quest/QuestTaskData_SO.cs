using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Task")]
public class QuestTaskData_SO : ScriptableObject
{
    public QuestTaskType type;

    [Header("Target")]
    public ItemData_SO targetItem;
    public int requiredAmount = 1;
}