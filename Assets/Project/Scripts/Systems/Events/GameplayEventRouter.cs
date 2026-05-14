/*
 * Arquitectura: Events
 * Script: GameplayEventRouter
 * Rol: Adapter central de integracion entre eventos de sistemas.
 * Relaciones: Traduce InventoryEvents/CraftingEvents hacia QuestEvents y GameplayEvents sin que Inventory o Crafting conozcan Quest/Notification.
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
        Uninstall();

        InventoryEvents.OnItemAdded += ForwardItemCollectedToQuest;
        InventoryEvents.OnNotificationRequested += ForwardNotification;
        CraftingEvents.OnItemCrafted += ForwardItemCraftedToQuest;
        CraftingEvents.OnItemRefined += ForwardItemRefinedToQuest;
        CraftingEvents.OnNotificationRequested += ForwardNotification;

        installed = true;
    }

    public static void Uninstall()
    {
        InventoryEvents.OnItemAdded -= ForwardItemCollectedToQuest;
        InventoryEvents.OnNotificationRequested -= ForwardNotification;
        CraftingEvents.OnItemCrafted -= ForwardItemCraftedToQuest;
        CraftingEvents.OnItemRefined -= ForwardItemRefinedToQuest;
        CraftingEvents.OnNotificationRequested -= ForwardNotification;

        installed = false;
    }

    private static void ForwardItemCollectedToQuest(ItemData_SO item, int amount)
    {
        QuestEvents.OnItemCollected?.Invoke(item, amount);
    }

    private static void ForwardItemCraftedToQuest(ItemData_SO item, int amount)
    {
        QuestEvents.OnItemCrafted?.Invoke(item, amount);
    }

    private static void ForwardItemRefinedToQuest(ItemData_SO item, int amount)
    {
        QuestEvents.OnItemRefined?.Invoke(item, amount);
    }

    private static void ForwardNotification(NotificationData notification)
    {
        GameplayEvents.OnNotification?.Invoke(notification);
    }
}
