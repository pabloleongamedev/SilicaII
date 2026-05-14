/*
 * Arquitectura: Interaction/Runtime
 * Script: ItemPickup
 * Rol: Interactable runtime de recoleccion. Convierte una interaccion del mundo en una operacion de Inventory.
 * Modulo: Gestiona deteccion, contexto y ejecucion de interacciones del jugador con objetos del mundo.
 * Relaciones: Recibe InteractionContext con IInventoryReadModel/IInventoryWriteModel; luego fuerza refresco de InteractionDetector mediante FindFirstObjectByType.
 * Riesgo arquitectonico: la operacion de inventario esta desacoplada, pero el refresco del detector usa busqueda global; debe pasar por contexto, evento o detector owner.
 * Uso como referencia: buen ejemplo parcial de IInteractable, con una dependencia de escena aun pendiente.
 */
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
