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

    private MovementController movementController;
    private PlayerCameraRig cameraRig;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        cameraRig = GetComponentInParent<PlayerCameraRig>();
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

        if (cameraRig != null)
            cameraRig.enabled = !isUI;

        // 🔥 ESTA LÍNEA ES LA QUE TE FALTABA
        UIStateEvents.Publish(currentState);
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
}
