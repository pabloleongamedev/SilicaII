using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Events/Inventory Event Channel")]
public class InventoryEventChannel_SO : ScriptableObject
{
    public event Action<ItemData_SO, int> ItemAdded;
    public event Action<ItemData_SO, int> ItemRemoved;
    public event Action<string, int> ItemAddedByID;
    public event Action<string, int> ItemRemovedByID;
    public event Action<NotificationData> NotificationRequested;

    public void RaiseItemAdded(ItemData_SO item, int amount)
    {
        ItemAdded?.Invoke(item, amount);
    }

    public void RaiseItemRemoved(ItemData_SO item, int amount)
    {
        ItemRemoved?.Invoke(item, amount);
    }

    public void RaiseItemAddedByID(string itemID, int amount)
    {
        ItemAddedByID?.Invoke(itemID, amount);
    }

    public void RaiseItemRemovedByID(string itemID, int amount)
    {
        ItemRemovedByID?.Invoke(itemID, amount);
    }

    public void RaiseNotification(NotificationData notification)
    {
        NotificationRequested?.Invoke(notification);
    }
}
