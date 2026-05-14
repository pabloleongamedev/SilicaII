/*
 * Arquitectura: Crafting/Runtime
 * Script: ChemistryTable
 * Rol: Interactable runtime que abre/cierra el estado UI de chemistry.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Implementa IInteractable; usa PlayerStateController para cambiar UIState.Chemistry; actualmente lo encuentra con FindFirstObjectByType.
 * Riesgo arquitectonico: el interactuable depende de un controlador global de Player; debe recibir estado UI por InteractionContext o referencia serializada/binder.
 * Uso como referencia: muestra una interaccion simple, pero evidencia el limite pendiente entre Interaction y Player UI State.
 */
using UnityEngine;

public class ChemistryTable : MonoBehaviour, IInteractable
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

        // 🔥 TOGGLE LIMPIO basado en estado global
        if (current == UIState.Chemistry)
        {
            playerState.SetState(UIState.None);
        }
        else if (current == UIState.None)
        {
            playerState.SetState(UIState.Chemistry);
        }
    }

    public string GetInteractionText()
    {
        if (playerState == null)
            return "Presiona E para usar reactor de ruptura";

        // basado en estado real, no en bool local
        return playerState.GetState() == UIState.Chemistry
            ? null
            : "Presiona E para usar reactor de ruptura";
    }
}
