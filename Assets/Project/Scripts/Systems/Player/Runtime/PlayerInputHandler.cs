using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("References")]
    private PlayerStateController stateController;
    private MovementController movementController;
    private MouseLook mouseLook;
    private InventoryController inventoryController;
    private InteractionDetector interactionDetector;

    private InputSystem_Actions inputActions;
    private InteractionContext interactionContext;
    private GameStateController gameStateController;


    private void Awake()
    {
        inventoryController = GetComponent<InventoryController>();
        stateController = GetComponent<PlayerStateController>();
        movementController = GetComponent<MovementController>();
        interactionDetector = GetComponent<InteractionDetector>();
        mouseLook = GetComponentInChildren<MouseLook>();
        gameStateController = GetComponent<GameStateController>();  
        inputActions = new InputSystem_Actions();
    }

    private void Start()
    {
        if (inventoryController == null || inventoryController.ReadModel == null || inventoryController.WriteModel == null)
        {
            Debug.LogError("InventorySystem NULL en Start");
            return;
        }

        interactionContext = new InteractionContext(inventoryController.ReadModel, inventoryController.WriteModel);
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;

        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;

        inputActions.Player.Jump.started += ctx =>
        {
            movementController.OnJumpStarted();
            NotifyAnyInput();
        };

        inputActions.Player.Jetpack.performed += ctx =>
        {
            movementController.SetJetpack(true);
            NotifyAnyInput();
        };

        inputActions.Player.Jetpack.canceled += ctx =>
        {
            movementController.SetJetpack(false);
        };

        inputActions.Player.Inventory.performed += ctx =>
        {
            ToggleInventory();
            NotifyAnyInput();
        };

        inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // =========================================================
    // 🔥 EVENT BRIDGE
    // =========================================================

    private void NotifyAnyInput()
    {
        GameplayEvents.OnAnyInput?.Invoke();
    }

    // =========================================================
    // INPUT HANDLERS
    // =========================================================

    private void ToggleInventory()
    {
        if (IsBlocked()) return;

        var current = stateController.GetState();

        if (current == UIState.Inventory)
        {
            stateController.SetState(UIState.None);
            return;
        }

        if (current == UIState.None)
        {
            stateController.SetState(UIState.Inventory);
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (IsBlocked()) return;

        if (!ctx.performed) return;

        NotifyAnyInput();

        var interactable = interactionDetector.CurrentInteractable;

        if (interactable == null) return;

        if (!stateController.CanInteract(interactable))
            return;

        interactable.Interact(interactionContext);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (IsBlocked()) return;

        var value = ctx.ReadValue<Vector2>();

        movementController.SetMoveInput(value);

        if (value != Vector2.zero)
            NotifyAnyInput();
    }
    private bool IsBlocked()
    {
        return gameStateController != null && gameStateController.IsBlocked();
    }
    private void OnSprint(InputAction.CallbackContext ctx)
    {
        if (IsBlocked()) return;

        bool pressed = ctx.ReadValueAsButton();

        movementController.SetSprint(pressed);

        if (pressed)
            NotifyAnyInput();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        if (IsBlocked()) return;
        var value = ctx.ReadValue<Vector2>();

        mouseLook.SetLookInput(value);

        if (value != Vector2.zero)
            NotifyAnyInput();
    }
}
