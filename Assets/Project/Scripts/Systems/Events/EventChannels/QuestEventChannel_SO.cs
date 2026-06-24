using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Events/Quest Event Channel")]
public class QuestEventChannel_SO : ScriptableObject
{
    public event Action<QuestData_SO> QuestLoaded;
    public event Action<int, int, int, bool> TaskUpdated;
    public event Action<int> QuestCompleted;
    public event Action<string, int> ItemCollected;
    public event Action<string, int> ItemRefined;
    public event Action<string, int> ItemCrafted;

    public void RaiseQuestLoaded(QuestData_SO questData) => QuestLoaded?.Invoke(questData);
    public void RaiseTaskUpdated(int taskIndex, int current, int required, bool completed) => TaskUpdated?.Invoke(taskIndex, current, required, completed);
    public void RaiseQuestCompleted(int questIndex) => QuestCompleted?.Invoke(questIndex);
    public void RaiseItemCollected(string itemID, int amount) => ItemCollected?.Invoke(itemID, amount);
    public void RaiseItemRefined(string itemID, int amount) => ItemRefined?.Invoke(itemID, amount);
    public void RaiseItemCrafted(string itemID, int amount) => ItemCrafted?.Invoke(itemID, amount);
}
