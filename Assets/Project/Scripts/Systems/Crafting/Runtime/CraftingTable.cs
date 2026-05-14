/*
 * Arquitectura: Crafting/Runtime
 * Script: CraftingTable
 * Rol: Interactable runtime que abre/cierra el estado UI de crafting.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Implementa IInteractable; usa PlayerStateController para cambiar UIState.Crafting; actualmente lo encuentra con FindFirstObjectByType.
 * Riesgo arquitectonico: el interactuable depende de un controlador global de Player; debe recibir estado UI por InteractionContext o referencia serializada/binder.
 * Uso como referencia: muestra una interaccion simple, pero evidencia el limite pendiente entre Interaction y Player UI State.
 */
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    private PlayerStateController playerState;

    private void Awake()
    {
        playerState = FindFirstObjectByType<PlayerStateController>();
    }

    public void Interact(InteractionContext context)
    {
        if (playerState == null) return;

        var current = playerState.GetState();

        // 🔥 toggle basado en estado global
        if (current == UIState.Crafting)
        {
            playerState.SetState(UIState.None);
        }
        else if (current == UIState.None)
        {
            playerState.SetState(UIState.Crafting);
        }
    }

    public string GetInteractionText()
    {
        if (playerState == null)
            return "Presiona E para usar sintetizador";

        return playerState.GetState() == UIState.Crafting
            ? null
            : "Presiona E para usar sintetizador";
    }
}
