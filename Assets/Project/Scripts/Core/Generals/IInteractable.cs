using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Se ejecuta cuando el jugador interactúa (E)
    /// </summary>
    void Interact(InteractionContext context);

    /// <summary>
    /// Texto que se muestra en UI (ej: "Presiona E para abrir")
    /// </summary>
    string GetInteractionText();
}