/*
 * Arquitectura: Crafting/Runtime
 * Script: ChemistryTable
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
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