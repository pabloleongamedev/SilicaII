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
