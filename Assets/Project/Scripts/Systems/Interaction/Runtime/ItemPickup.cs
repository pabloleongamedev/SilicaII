/*
 * Arquitectura: Interaction/Runtime
 * Script: ItemPickup
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona deteccion, contexto y ejecucion de interacciones del jugador con objetos del mundo.
 * Relaciones: Usa IInteractable e InteractionContext para conectar jugador, mundo e Inventory sin dependencias profundas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
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