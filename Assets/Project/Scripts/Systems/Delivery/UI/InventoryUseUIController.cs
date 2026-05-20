/*
 * Arquitectura: Delivery/UI
 * Script: InventoryUseUIController
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona la entrega/uso de items desde la UI de inventario hacia objetos interactuables.
 * Relaciones: Usa Inventory y PlayerState para seleccionar y consumir items entregados.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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

        NotificationEvents.PublishNotification(new NotificationData
        {
            message = msg,
            type = type
        });
    }
}
