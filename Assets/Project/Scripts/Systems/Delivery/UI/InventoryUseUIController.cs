using UnityEngine;

public class InventoryUseUIController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InventoryListView inventoryListView;
/*
    public void OnUseButtonPressed()
    {
        if (inventoryListView == null)
        {
            Debug.LogError("[InventoryUseUI] InventoryListView no asignado");
            return;
        }

        var itemInstance = inventoryListView.GetSelectedItem();

        if (itemInstance == null)
        {
            Notify("No hay item seleccionado", NotificationType.Warning);
            return;
        }

        if (itemInstance.IsEmpty())
        {
            Notify("El item no tiene cantidad", NotificationType.Warning);
            return;
        }

        // 🔥 USO REAL
        DeliveryBoxInteractable.OnUseButtonPressed(
            itemInstance.Data,
            1
        );
    }
*/
    // =========================================================
    private void Notify(string msg, NotificationType type)
    {
        Debug.Log("[InventoryUseUI] " + msg);

        GameplayEvents.OnNotification?.Invoke(new NotificationData
        {
            message = msg,
            type = type
        });
    }
}