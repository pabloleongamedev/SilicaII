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