using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Events/Crafting Event Channel")]
public class CraftingEventChannel_SO : ScriptableObject
{
    public event Action<ItemData_SO, int> ItemCrafted;
    public event Action<ItemData_SO, int> ItemRefined;
    public event Action<string, int> ItemCraftedByID;
    public event Action<string, int> ItemRefinedByID;
    public event Action<NotificationData> NotificationRequested;

    public void RaiseItemCrafted(ItemData_SO item, int amount)
    {
        ItemCrafted?.Invoke(item, amount);
    }

    public void RaiseItemRefined(ItemData_SO item, int amount)
    {
        ItemRefined?.Invoke(item, amount);
    }

    public void RaiseItemCraftedByID(string itemID, int amount)
    {
        ItemCraftedByID?.Invoke(itemID, amount);
    }

    public void RaiseItemRefinedByID(string itemID, int amount)
    {
        ItemRefinedByID?.Invoke(itemID, amount);
    }

    public void RaiseNotification(NotificationData notification)
    {
        NotificationRequested?.Invoke(notification);
    }
}
