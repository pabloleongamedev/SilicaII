/*
 * Arquitectura: Player/Runtime
 * Script: PlayerStateController
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona estado global del jugador, input y bloqueos de gameplay/UI.
 * Relaciones: Coordina input, estados UI/gameplay y bloqueos globales usados por otros sistemas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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
    [SerializeField] private UIStateEventChannel_SO uiStateChannel;
    [SerializeField] private MonoBehaviour pauseServiceBehaviour;

    private MovementController movementController;
    private PlayerCameraRig cameraRig;
    private IGamePauseService pauseService;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        cameraRig = GetComponentInParent<PlayerCameraRig>();
        ResolvePauseService();
    }

    public void SetState(UIState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        bool isUI = newState != UIState.None;

        Cursor.lockState = isUI ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isUI;

        if (pauseService != null)
            pauseService.SetPaused(isUI);

        if (movementController != null)
            movementController.SetInputEnabled(!isUI);

        if (cameraRig != null)
            cameraRig.enabled = !isUI;

        uiStateChannel?.Raise(currentState);
    }

    public UIState GetState() => currentState;

    public bool CanInteract(IInteractable interactable)
    {
        if (interactable == null)
            return false;

        if (interactable is IUIStateInteractable stateAware)
            return stateAware.CanInteractInState(currentState);

        switch (currentState)
        {
            case UIState.None:
                return true;
            case UIState.Inventory:
                return false;
            default:
                return false;
        }
    }

    private void ResolvePauseService()
    {
        pauseService = pauseServiceBehaviour as IGamePauseService;

        if (pauseServiceBehaviour != null && pauseService == null)
            Debug.LogWarning("[PlayerStateController] El Pause Service asignado no implementa IGamePauseService.", this);

        if (pauseService == null)
            Debug.LogWarning("[PlayerStateController] Asigna un GamePauseService por Inspector.", this);
    }
}
