/*
 * Arquitectura: Crafting/Runtime
 * Script: ChemistryTable
 * Rol: Interactable runtime que abre/cierra el estado UI de chemistry.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Implementa IInteractable; usa PlayerStateController asignado por Inspector para cambiar UIState.Chemistry.
 * Riesgo arquitectonico mitigado: elimina busqueda global; la dependencia de estado UI queda visible en escena.
 * Uso como referencia: muestra una interaccion simple, pero evidencia el limite pendiente entre Interaction y Player UI State.
 */
using UnityEngine;

public class ChemistryTable : MonoBehaviour, IInteractable, IUIStateInteractable
{
    [SerializeField] private PlayerStateController playerState;

    public void Interact(InteractionContext context)
    {
        if (playerState == null) return;

        var current = playerState.GetState();

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

    public bool CanInteractInState(UIState currentState)
    {
        return currentState == UIState.None || currentState == UIState.Chemistry;
    }
}
