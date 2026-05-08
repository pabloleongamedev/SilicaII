using UnityEngine;
using System.Collections.Generic;

public class QuestSystem : MonoBehaviour
{
    [SerializeField] private List<QuestData_SO> quests;
    public static event System.Action<int> OnQuestCompleted;

    private int currentQuestIndex = 0;
    private QuestData_SO currentQuest;

    private Dictionary<int, int> progress = new();

    private bool isInitialized = false;

    private void Start()
    {
        LoadQuest(0);
    }

    private void OnEnable()
    {
        QuestEvents.OnItemCollected += HandleCollect;
        QuestEvents.OnItemRefined += HandleRefined;
        QuestEvents.OnItemCrafted += HandleCraft;
    }

    private void OnDisable()
    {
        QuestEvents.OnItemCollected -= HandleCollect;
        QuestEvents.OnItemRefined -= HandleRefined;
        QuestEvents.OnItemCrafted -= HandleCraft;
    }

    // =========================================
    private void LoadQuest(int index)
    {
        if (index >= quests.Count)
        {
            Debug.Log("TODAS LAS MISIONES COMPLETADAS");
            return;
        }

        currentQuestIndex = index;
        currentQuest = quests[index];

        progress.Clear();

        for (int i = 0; i < currentQuest.tasks.Count; i++)
            progress[i] = 0;

        isInitialized = true;

        // 🔥 UI
        QuestEvents.OnQuestLoaded?.Invoke(currentQuest);
    }

    // =========================================
    private void HandleCollect(ItemData_SO item, int amount)
    {
        if (!isInitialized) return;
        UpdateTasks(item, amount, QuestTaskType.Collect);
    }

    private void HandleRefined(ItemData_SO item, int amount)
    {
        if (!isInitialized) return;
        UpdateTasks(item, amount, QuestTaskType.Refine);
    }

    private void HandleCraft(ItemData_SO item, int amount)
    {
        if (!isInitialized) return;
        UpdateTasks(item, amount, QuestTaskType.Craft);
    }

    // =========================================
    private void UpdateTasks(ItemData_SO item, int amount, QuestTaskType type)
    {
        if (currentQuest == null) return;
        if (currentQuest.tasks == null) return;

        for (int i = 0; i < currentQuest.tasks.Count; i++)
        {
            var task = currentQuest.tasks[i];

            if (task == null || task.targetItem == null)
                continue;

            if (task.type != type) continue;
            if (task.targetItem != item) continue;

            int required = task.requiredAmount;
            progress[i] = Mathf.Min(progress[i] + amount, required);

            int current = progress[i];

            bool completed = current >= required;

            QuestEvents.OnTaskUpdated?.Invoke(i, current, required, completed);
        }

        CheckQuestComplete();
    }
    private void CheckQuestComplete()
    {
        for (int i = 0; i < currentQuest.tasks.Count; i++)
        {
            if (progress[i] < currentQuest.tasks[i].requiredAmount)
                return;
        }

        Debug.Log("MISIÓN COMPLETADA");

        // 🔥 ESTE ES EL PUNTO CLAVE
        QuestEvents.OnQuestCompleted?.Invoke(currentQuestIndex);
        OnQuestCompleted?.Invoke(currentQuestIndex);

        LoadQuest(currentQuestIndex + 1);
    }
    public QuestData_SO GetCurrentQuest()
    {
        return currentQuest;
    }

    public int GetTaskProgress(int index)
    {
        if (progress.ContainsKey(index))
            return progress[index];

        return 0;
    }

}
