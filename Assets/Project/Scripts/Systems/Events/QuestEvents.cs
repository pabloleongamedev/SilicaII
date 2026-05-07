using System;

public static class QuestEvents
{
    // 🔥 SISTEMA → UI
    public static Action<QuestData_SO> OnQuestLoaded;

    public static Action<int, int, int, bool> OnTaskUpdated;
    // index, current, required, completed

    // 🔥 GAMEPLAY → SISTEMA
    public static Action<ItemData_SO, int> OnItemCollected;
    
    public static Action<ItemData_SO, int> OnItemRefined;
    public static Action<ItemData_SO, int> OnItemCrafted;

}