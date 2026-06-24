/*
 * Arquitectura: Player/Runtime
 * Script: PlayerInputHandler
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona estado global del jugador, input y bloqueos de gameplay/UI.
 * Relaciones: Coordina input, estados UI/gameplay y bloqueos globales usados por otros sistemas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStateController stateControllerBehaviour;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private MovementController movementController;
    [SerializeField] private InteractionDetector interactionDetector;
    [SerializeField] private PlayerCameraRig cameraRig;
    [SerializeField] private PlayerPerspectiveController perspectiveController;
    [SerializeField] private GameStateController gameStateController;

    private PlayerStateController stateController;

    private InputSystem_Actions inputActions;
    private InteractionContext interactionContext;
    private bool inputCallbacksBound;


    private void Awake()
    {
        // Runtime bridge del jugador: localiza los controladores que reciben input.
        // Este script no contiene reglas de Inventory/Crafting/Quest; solo traduce
        // acciones del Input System a llamadas sobre facades y controladores.
        ResolveReferences();
        CreateInputActionsIfNeeded();
    }

    private void ResolveReferences()
    {
        stateController = stateControllerBehaviour;
        WarnMissing(inventoryController, "InventoryController");
        WarnMissing(stateController, "PlayerStateController");
        WarnMissing(movementController, "MovementController");
        WarnMissing(interactionDetector, "InteractionDetector");
        WarnMissing(cameraRig, "PlayerCameraRig");
        WarnMissing(gameStateController, "GameStateController");
    }

    private void CreateInputActionsIfNeeded()
    {
        if (inputActions != null)
            return;

        inputActions = new InputSystem_Actions();
        BindInputCallbacks();
    }

    private void Start()
    {
        // El contexto de interaccion se construye con interfaces de Inventory.
        // Asi los interactuables no conocen InventorySystem ni sus detalles internos.
        if (inventoryController == null || inventoryController.ReadModel == null || inventoryController.WriteModel == null)
        {
            Debug.LogWarning("[PlayerInputHandler] Asigna InventoryController por Inspector para construir InteractionContext.", this);
            return;
        }

        interactionContext = new InteractionContext(inventoryController.ReadModel, inventoryController.WriteModel);
    }

    private void OnEnable()
    {
        CreateInputActionsIfNeeded();

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnDestroy()
    {
        UnbindInputCallbacks();
    }

    // =========================================================
    // 🔥 EVENT BRIDGE
    // =========================================================

    private void NotifyAnyInput()
    {
        // Punto de extension reservado para un InputActivityEventChannel_SO si se requiere telemetria de input.
    }

    // =========================================================
    // INPUT HANDLERS
    // =========================================================

    private void ToggleInventory()
    {
        // PlayerStateController es la fuente de verdad para pantallas abiertas.
        // Los paneles escuchan UIStateEventChannel_SO para mostrarse.
        if (IsBlocked()) return;

        if (stateController == null)
        {
            Debug.LogWarning("[PlayerInputHandler] No hay PlayerStateController asignado para ToggleInventory.", this);
            return;
        }

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
        // Flujo: input -> detector -> IInteractable.Interact(context).
        // Esto desacopla al jugador de cada implementacion concreta del mundo.
        if (IsBlocked()) return;

        if (!ctx.performed) return;

        NotifyAnyInput();

        if (interactionDetector == null)
        {
            Debug.LogWarning("[PlayerInputHandler] InteractionDetector no esta asignado o no existe en el arbol del Player.", this);
            return;
        }

        var interactable = interactionDetector.CurrentInteractable;

        if (interactable == null) return;

        if (stateController != null && !stateController.CanInteract(interactable))
            return;

        if (interactionContext == null)
        {
            Debug.LogWarning("[PlayerInputHandler] InteractionContext no esta inicializado; revisa InventoryController en el Player.", this);
            return;
        }

        interactable.Interact(interactionContext);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (IsBlocked()) return;

        var value = ctx.ReadValue<Vector2>();

        if (movementController == null)
        {
            Debug.LogWarning("[PlayerInputHandler] MovementController no esta asignado o no existe en el arbol del Player.", this);
            return;
        }

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

        if (movementController == null)
        {
            Debug.LogWarning("[PlayerInputHandler] MovementController no esta asignado para Sprint.", this);
            return;
        }

        movementController.SetSprint(pressed);

        if (pressed)
            NotifyAnyInput();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        if (IsBlocked()) return;
        var value = ctx.ReadValue<Vector2>();

        if (cameraRig == null)
        {
            Debug.LogWarning("[PlayerInputHandler] PlayerCameraRig no esta asignado para Look.", this);
            return;
        }

        cameraRig.SetLookInput(value);

        if (value != Vector2.zero)
            NotifyAnyInput();
    }

    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        if (movementController == null)
        {
            Debug.LogWarning("[PlayerInputHandler] MovementController no esta asignado para Jump.", this);
            return;
        }

        movementController.OnJumpStarted();
        NotifyAnyInput();
    }

    private void OnJetpackPerformed(InputAction.CallbackContext ctx)
    {
        if (movementController == null)
        {
            Debug.LogWarning("[PlayerInputHandler] MovementController no esta asignado para Jetpack.", this);
            return;
        }

        movementController.SetJetpack(true);
        NotifyAnyInput();
    }

    private void OnJetpackCanceled(InputAction.CallbackContext ctx)
    {
        if (movementController == null)
            return;

        movementController.SetJetpack(false);
    }

    private void OnInventoryPerformed(InputAction.CallbackContext ctx)
    {
        ToggleInventory();
        NotifyAnyInput();
    }

    private void OnCameraPerformed(InputAction.CallbackContext ctx)
    {
        if (IsBlocked()) return;
        if (!ctx.performed) return;

        if (perspectiveController == null)
        {
            Debug.LogWarning("[PlayerInputHandler] PlayerPerspectiveController no esta asignado para alternar camara.", this);
            return;
        }

        perspectiveController.TogglePerspective();
        NotifyAnyInput();
    }

    private void BindInputCallbacks()
    {
        if (inputCallbacksBound || inputActions == null)
            return;

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
        inputActions.Player.Jump.started += OnJumpStarted;
        inputActions.Player.Jetpack.performed += OnJetpackPerformed;
        inputActions.Player.Jetpack.canceled += OnJetpackCanceled;
        inputActions.Player.Camera.performed += OnCameraPerformed;
        inputActions.Player.Inventory.performed += OnInventoryPerformed;
        inputActions.Player.Interact.performed += OnInteract;
        inputCallbacksBound = true;
    }

    private void UnbindInputCallbacks()
    {
        if (!inputCallbacksBound)
            return;

        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLook;
        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;
        inputActions.Player.Jump.started -= OnJumpStarted;
        inputActions.Player.Jetpack.performed -= OnJetpackPerformed;
        inputActions.Player.Jetpack.canceled -= OnJetpackCanceled;
        inputActions.Player.Camera.performed -= OnCameraPerformed;
        inputActions.Player.Inventory.performed -= OnInventoryPerformed;
        inputActions.Player.Interact.performed -= OnInteract;
        inputCallbacksBound = false;
    }

    private void WarnMissing(Object reference, string referenceName)
    {
        if (reference == null)
            Debug.LogWarning($"[PlayerInputHandler] Asigna {referenceName} por Inspector.", this);
    }
}
