/*
 * Arquitectura: Events/EventChannels
 * Script: GameplayEventChannelsBinding
 * Rol: Router de escena entre EventChannels ScriptableObject.
 * Relaciones: Conecta Inventory/Crafting con Quest/Notification sin buses estaticos ni dependencias directas entre sistemas.
 * Uso como referencia: mantener este binding en escena hace visibles las reglas de integracion entre canales.
 */
using UnityEngine;

public class GameplayEventChannelsBinding : MonoBehaviour
{
    [Header("Notification")]
    [SerializeField] private NotificationEventChannel_SO notificationChannel;

    [Header("Gameplay")]
    [SerializeField] private InventoryEventChannel_SO inventoryChannel;
    [SerializeField] private CraftingEventChannel_SO craftingChannel;
    [SerializeField] private QuestEventChannel_SO questChannel;

    private void OnEnable()
    {
        if (inventoryChannel != null)
        {
            inventoryChannel.ItemAddedByID += ForwardItemCollectedToQuest;
            inventoryChannel.NotificationRequested += ForwardNotification;
        }

        if (craftingChannel != null)
        {
            craftingChannel.ItemCraftedByID += ForwardItemCraftedToQuest;
            craftingChannel.ItemRefinedByID += ForwardItemRefinedToQuest;
            craftingChannel.NotificationRequested += ForwardNotification;
        }
    }

    private void OnDisable()
    {
        if (inventoryChannel != null)
        {
            inventoryChannel.ItemAddedByID -= ForwardItemCollectedToQuest;
            inventoryChannel.NotificationRequested -= ForwardNotification;
        }

        if (craftingChannel != null)
        {
            craftingChannel.ItemCraftedByID -= ForwardItemCraftedToQuest;
            craftingChannel.ItemRefinedByID -= ForwardItemRefinedToQuest;
            craftingChannel.NotificationRequested -= ForwardNotification;
        }
    }

    private void ForwardItemCollectedToQuest(string itemID, int amount)
    {
        questChannel?.RaiseItemCollected(itemID, amount);
    }

    private void ForwardItemCraftedToQuest(string itemID, int amount)
    {
        questChannel?.RaiseItemCrafted(itemID, amount);
    }

    private void ForwardItemRefinedToQuest(string itemID, int amount)
    {
        questChannel?.RaiseItemRefined(itemID, amount);
    }

    private void ForwardNotification(NotificationData notification)
    {
        notificationChannel?.Raise(notification);
    }
}
