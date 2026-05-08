using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("First Selection Buttons")]
    [SerializeField] private Button mainFirstButton;
    [SerializeField] private Button playFirstButton;
    [SerializeField] private Button optionsFirstButton;
    [SerializeField] private Button creditsFirstButton;

    [Header("Loading")]
    [SerializeField] private GameObject loadingImage;
    [SerializeField] private float delay = 4f;
    [SerializeField] private int fallbackSceneIndex = 1;

    private const string UniqueSlot = "1";

    private void Start()
    {
        EnsureGameManager();
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        SwitchPanel(mainPanel, mainFirstButton);
    }

    public void ShowPlayMenu()
    {
        EnsureGameManager();

        if (playPanel != null)
        {
            SwitchPanel(playPanel, playFirstButton);

            var saveSlot = playPanel.GetComponentInChildren<SaveSlot>(true);
            if (saveSlot != null)
                saveSlot.RefreshSlot();
        }
    }

    public void ShowOptions()
    {
        SwitchPanel(optionsPanel, optionsFirstButton);
    }

    public void ShowCredits()
    {
        SwitchPanel(creditsPanel, creditsFirstButton);
    }

    public void StartGame()
    {
        EnsureGameManager();

        if (GameManager.Instance.HasSaveFile(UniqueSlot))
            GameManager.Instance.LoadGame(UniqueSlot);
        else
            GameManager.Instance.CreateNewGame(UniqueSlot);
    }

    public void LoadSceneFromButton()
    {
        StartCoroutine(LoadSceneRoutine());
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadSceneRoutine()
    {
        if (loadingImage != null)
            loadingImage.SetActive(true);

        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(fallbackSceneIndex);
    }

    private void SwitchPanel(GameObject targetPanel, Button firstButton)
    {
        SetPanel(mainPanel, false);
        SetPanel(playPanel, false);
        SetPanel(optionsPanel, false);
        SetPanel(creditsPanel, false);
        SetPanel(targetPanel, true);

        if (firstButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            firstButton.Select();
        }
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    private void EnsureGameManager()
    {
        if (GameManager.Instance != null)
            return;

        var gameManagerObject = new GameObject("GameManager");
        gameManagerObject.AddComponent<GameManager>();
    }
}
