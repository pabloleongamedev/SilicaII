using System;

public static class InventoryEvents
{
    public static Action<ItemData_SO, int> OnItemAdded;
    public static Action<ItemData_SO, int> OnItemRemoved;
    public static Action<NotificationData> OnNotificationRequested;
}
