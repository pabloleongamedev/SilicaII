using System;

public static class CraftingEvents
{
    public static Action<ItemData_SO, int> OnItemCrafted;
    public static Action<ItemData_SO, int> OnItemRefined;
    public static Action<NotificationData> OnNotificationRequested;
}
