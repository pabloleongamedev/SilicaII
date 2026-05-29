/*
 * Arquitectura: Menu/UI
 * Script: MainMenuManager
 * Rol: Controlador UI de menu principal y flujo inicial de partida.
 * Modulo: Gestiona paneles, seleccion inicial y entrada al flujo de partida.
 * Relaciones: Usa ISaveSlotReader/IGameSessionLoader e ISceneLoader asignables por Inspector; SaveSlot muestra el estado del slot.
 * Riesgo arquitectonico mitigado: no conoce la implementacion concreta de SaveLoad; requiere asignar servicios por Inspector.
 * Uso como referencia: la UI delega casos de uso y evita conocer SaveController, disco o GameData.
 */
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject bannerPanel;
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

    [Header("SaveLoad Use Cases")]
    [SerializeField] private MonoBehaviour saveSlotReaderBehaviour;
    [SerializeField] private MonoBehaviour gameSessionLoaderBehaviour;

    [Header("Scene Loading")]
    [SerializeField] private MonoBehaviour sceneLoaderBehaviour;

    private const string UniqueSlot = "1";

    private ISaveSlotReader saveSlotReader;
    private IGameSessionLoader gameSessionLoader;
    private ISceneLoader sceneLoader;

    private void Awake()
    {
        saveSlotReader = saveSlotReaderBehaviour as ISaveSlotReader;
        gameSessionLoader = gameSessionLoaderBehaviour as IGameSessionLoader;
        sceneLoader = ResolveSceneLoader();
    }

    private void Start()
    {
        ResolveSerializedServices();
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        SetPanel(mainPanel, true);
        SetPanel(bannerPanel, true);
        SetPanel(playPanel, false);
        SetPanel(optionsPanel, false);
        SetPanel(creditsPanel, false);
        SelectButton(mainFirstButton);
    }

    public void ShowPlayMenu()
    {
        ResolveSerializedServices();

        if (playPanel == null)
            return;

        SetPanel(mainPanel, true);
        SetPanel(bannerPanel, true);
        SetPanel(playPanel, true);
        SetPanel(optionsPanel, false);
        SetPanel(creditsPanel, false);
        SelectButton(playFirstButton);

        foreach (var saveSlot in playPanel.GetComponentsInChildren<SaveSlot>(true))
            saveSlot.RefreshSlot();
    }

    public void ShowOptions()
    {
        SetPanel(mainPanel, true);
        SetPanel(bannerPanel, false);
        SetPanel(playPanel, false);
        SetPanel(optionsPanel, true);
        SetPanel(creditsPanel, false);
        SelectButton(optionsFirstButton);
    }

    public void ShowCredits()
    {
        if (creditsPanel == null)
        {
            Debug.LogWarning("[MainMenuManager] Asigna Credits Panel por Inspector para mostrar creditos.", this);
            ShowMainMenu();
            return;
        }

        SetPanel(mainPanel, true);
        SetPanel(bannerPanel, false);
        SetPanel(playPanel, false);
        SetPanel(optionsPanel, false);
        SetPanel(creditsPanel, true);
        SelectButton(creditsFirstButton);
    }

    public void StartGame()
    {
        ResolveSerializedServices();

        if (saveSlotReader == null || gameSessionLoader == null)
        {
            Debug.LogWarning("[MainMenuManager] Faltan ISaveSlotReader/IGameSessionLoader para iniciar partida.", this);
            return;
        }

        if (saveSlotReader.HasSaveFile(UniqueSlot))
            gameSessionLoader.LoadGame(UniqueSlot);
        else
            gameSessionLoader.CreateNewGame(UniqueSlot);
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
        ResolveSceneLoader().LoadScene(fallbackSceneIndex);
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    private void SelectButton(Button button)
    {
        if (button == null || EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);
        button.Select();
    }

    private void ResolveSerializedServices()
    {
        if (saveSlotReader == null)
            saveSlotReader = saveSlotReaderBehaviour as ISaveSlotReader;

        if (gameSessionLoader == null)
            gameSessionLoader = gameSessionLoaderBehaviour as IGameSessionLoader;

        if (saveSlotReader == null || gameSessionLoader == null)
            Debug.LogWarning("[MainMenuManager] Asigna ISaveSlotReader e IGameSessionLoader por Inspector para desacoplar Menu de SaveLoad.", this);
    }

    private ISceneLoader ResolveSceneLoader()
    {
        if (sceneLoader == null)
            sceneLoader = sceneLoaderBehaviour as ISceneLoader;

        if (sceneLoader == null && sceneLoaderBehaviour != null)
            Debug.LogWarning("[MainMenuManager] El Scene Loader asignado no implementa ISceneLoader.", this);

        if (sceneLoader == null)
            sceneLoader = new UnitySceneLoader();

        return sceneLoader;
    }
}
