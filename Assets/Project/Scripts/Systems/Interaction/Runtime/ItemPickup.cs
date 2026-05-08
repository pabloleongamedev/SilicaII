using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData_SO item;
    [SerializeField] private int amount = 1;

    public void Interact(InteractionContext context)
    {
        if (item == null)
            return;

        if (!context.InventoryRead.CanAddItemsBatch((item, amount)))
            return;

        context.InventoryWrite.AddItem(item, amount);

        // IMPORTANTE: limpiar interacción ANTES de destruir
        var detector = FindFirstObjectByType<InteractionDetector>();
        if (detector != null)
            detector.ForceRefresh();

        Destroy(gameObject);
    }
    public string GetInteractionText()
    {
        return item != null
            ? $"Presiona E para recoger {item.itemID}"
            : "Recoger objeto";
    }
}