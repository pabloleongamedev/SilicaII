/*
 * Arquitectura: Menu/UI
 * Script: PauseMenuManager
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Usa ISceneLoader para volver al menu sin depender de SceneManager directo.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject optionsPanelUI;
    bool isPaused = false;
    public bool IsPaused => isPaused;

    [SerializeField] private PlayerStateController playerStateController;
    [SerializeField] private MonoBehaviour pauseServiceBehaviour;
    [SerializeField] private MonoBehaviour sceneLoaderBehaviour;
    [SerializeField] private string menuSceneName = "Menu";

    private InputSystem_Actions inputActions;
    private ISceneLoader sceneLoader;
    private IGamePauseService pauseService;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        ResolveSceneLoader();
        ResolvePauseService();
        
    }

    void OnEnable()
    {
        inputActions.UI.Pause.performed += OnPausePressed;
        inputActions.UI.Enable();
    }

    void OnDisable()
    {
        inputActions.UI.Pause.performed -= OnPausePressed;
        inputActions.UI.Disable();
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        optionsPanelUI.SetActive(false);
        ResolvePauseService()?.SetPaused(true);
        if (playerStateController != null)
            playerStateController.SetState(UIState.Blocked);
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        optionsPanelUI.SetActive(false); // cerrar el panel de opciones
        ResolvePauseService()?.SetPaused(false);
        if (playerStateController != null)
            playerStateController.SetState(UIState.None);
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        ResolvePauseService()?.SetPaused(false);
        if (playerStateController != null)
            playerStateController.SetState(UIState.None);

        var loader = ResolveSceneLoader();
        if (loader == null)
            return;

        loader.LoadScene(menuSceneName);
    }

    public void OpenOptions()
    {

        pauseMenuUI.transform.Find("Panel").gameObject.SetActive(false);
        optionsPanelUI.SetActive(true);
    }

        public void CloseOptions()
    {
        optionsPanelUI.SetActive(false);
        pauseMenuUI.transform.Find("Panel").gameObject.SetActive(true);
        ResumeGame();
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    private ISceneLoader ResolveSceneLoader()
    {
        if (sceneLoader == null)
            sceneLoader = sceneLoaderBehaviour as ISceneLoader;

        if (sceneLoader == null && sceneLoaderBehaviour != null)
            Debug.LogWarning("[PauseMenuManager] El Scene Loader asignado no implementa ISceneLoader.", this);

        if (sceneLoader == null)
            Debug.LogWarning("[PauseMenuManager] Asigna un SceneLoadService por Inspector.", this);

        return sceneLoader;
    }

    private IGamePauseService ResolvePauseService()
    {
        if (pauseService == null)
            pauseService = pauseServiceBehaviour as IGamePauseService;

        if (pauseService == null && pauseServiceBehaviour != null)
            Debug.LogWarning("[PauseMenuManager] El Pause Service asignado no implementa IGamePauseService.", this);

        if (pauseService == null)
            Debug.LogWarning("[PauseMenuManager] Asigna un GamePauseService por Inspector.", this);

        return pauseService;
    }
}
