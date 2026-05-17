/*
 * Arquitectura: Interaction/Runtime
 * Script: ItemPickup
 * Rol: Interactable runtime de recoleccion. Convierte una interaccion del mundo en una operacion de Inventory.
 * Modulo: Gestiona deteccion, contexto y ejecucion de interacciones del jugador con objetos del mundo.
 * Relaciones: Recibe InteractionContext con IInventoryReadModel/IInventoryWriteModel y no conoce detector, Player ni UI.
 * Riesgo arquitectonico mitigado: elimina busqueda global; el detector debe reaccionar por su ciclo normal al destruirse el objeto.
 * Uso como referencia: IInteractable recibe dependencias por contexto y ejecuta una operacion de dominio pequena.
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
        Destroy(gameObject);
    }
    public string GetInteractionText()
    {
        return item != null
            ? $"Presiona E para recoger {item.itemID}"
            : "Recoger objeto";
    }
}
