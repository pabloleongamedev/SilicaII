using UnityEngine;
using UnityEngine.InputSystem;
using System;

[Obsolete("Usar PlayerInputHandler como entrada unica de interaccion. Este componente queda solo para compatibilidad de escenas antiguas.")]
public class InteractionController : MonoBehaviour
{
    [SerializeField] private InteractionDetector detector;
    [SerializeField] private InventoryController inventoryController;

    private InteractionContext context;

    private void Start()
    {
        if (inventoryController == null)
        {
            Debug.LogError("InventoryController no asignado");
            return;
        }

        context = new InteractionContext(inventoryController.ReadModel, inventoryController.WriteModel);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        var interactable = detector.CurrentInteractable;

        if (interactable != null)
        {
            interactable.Interact(context);
        }
    }
}
