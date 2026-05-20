/*
 * Arquitectura: Events
 * Script: GameplayEventRouter
 * Rol: Adapter central de integracion entre eventos de sistemas.
 * Relaciones: Conecta InventoryEvents/CraftingEvents por itemID hacia QuestEvents y NotificationEvents sin que Inventory o Crafting conozcan Quest/Notification.
 * Riesgo arquitectonico mitigado: concentra el acoplamiento transversal en un unico router reemplazable por servicios/event channels en fases futuras.
 */
using UnityEngine;

public static class GameplayEventRouter
{
    private static bool installed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        installed = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InstallOnLoad()
    {
        Install();
    }

    public static void Install()
    {
        if (installed)
            Uninstall();

        InventoryEvents.OnItemAddedByID += ForwardItemCollectedToQuest;
        InventoryEvents.OnNotificationRequested += ForwardNotification;
        CraftingEvents.OnItemCraftedByID += ForwardItemCraftedToQuest;
        CraftingEvents.OnItemRefinedByID += ForwardItemRefinedToQuest;
        CraftingEvents.OnNotificationRequested += ForwardNotification;

        installed = true;
    }

    public static void Uninstall()
    {
        if (!installed)
            return;

        InventoryEvents.OnItemAddedByID -= ForwardItemCollectedToQuest;
        InventoryEvents.OnNotificationRequested -= ForwardNotification;
        CraftingEvents.OnItemCraftedByID -= ForwardItemCraftedToQuest;
        CraftingEvents.OnItemRefinedByID -= ForwardItemRefinedToQuest;
        CraftingEvents.OnNotificationRequested -= ForwardNotification;

        installed = false;
    }

    private static void ForwardItemCollectedToQuest(string itemID, int amount)
    {
        if (!string.IsNullOrEmpty(itemID))
            QuestEvents.PublishItemCollected(itemID, amount);
    }

    private static void ForwardItemCraftedToQuest(string itemID, int amount)
    {
        if (!string.IsNullOrEmpty(itemID))
            QuestEvents.PublishItemCrafted(itemID, amount);
    }

    private static void ForwardItemRefinedToQuest(string itemID, int amount)
    {
        if (!string.IsNullOrEmpty(itemID))
            QuestEvents.PublishItemRefined(itemID, amount);
    }

    private static void ForwardNotification(NotificationData notification)
    {
        NotificationEvents.PublishNotification(notification);
    }
}
