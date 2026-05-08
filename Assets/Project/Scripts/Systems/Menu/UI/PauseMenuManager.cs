using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject optionsPanelUI;
    bool isPaused = false;
    public bool IsPaused => isPaused;

    [SerializeField] private PlayerStateController playerStateController;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        
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
        Time.timeScale = 0f;
        if (playerStateController != null)
            playerStateController.SetState(UIState.Blocked);
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        optionsPanelUI.SetActive(false); // cerrar el panel de opciones
        Time.timeScale = 1f;
        if (playerStateController != null)
            playerStateController.SetState(UIState.None);
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        if (playerStateController != null)
            playerStateController.SetState(UIState.None);
        SceneManager.LoadScene("Menu");
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
}
