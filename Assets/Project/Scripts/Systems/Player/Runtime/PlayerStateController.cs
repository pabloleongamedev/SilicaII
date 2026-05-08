using UnityEngine;

public enum UIState
{
    None,
    Inventory,
    Crafting,
    Quest,
    Chemistry,Blocked 
}
public class PlayerStateController : MonoBehaviour
{
    private UIState currentState = UIState.None;

    private MovementController movementController;
    private MouseLook mouseLook;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        mouseLook = GetComponentInChildren<MouseLook>();
    }

    public void SetState(UIState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        bool isUI = newState != UIState.None;

        Cursor.lockState = isUI ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isUI;

        Time.timeScale = isUI ? 0f : 1f;

        if (movementController != null)
            movementController.SetInputEnabled(!isUI);

        if (mouseLook != null)
            mouseLook.enabled = !isUI;

        // 🔥 ESTA LÍNEA ES LA QUE TE FALTABA
        GameplayEvents.OnUIStateChanged?.Invoke(currentState);
    }

    public UIState GetState() => currentState;

    public bool CanInteract(IInteractable interactable)
    {
        switch (currentState)
        {
            case UIState.None:
                return true;

            case UIState.Inventory:
                return false;

            case UIState.Crafting:
                return interactable is CraftingTable;

            case UIState.Chemistry:
                return interactable is ChemistryTable;

            default:
                return false;
        }
    }
}