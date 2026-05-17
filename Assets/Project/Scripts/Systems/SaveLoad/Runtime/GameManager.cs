/*
 * Arquitectura: SaveLoad/Runtime
 * Script: GameManager
 * Rol: Fachada global temporal de SaveLoad. Mantiene compatibilidad con escenas actuales y delega reglas a servicios pequenos.
 * Modulo: Gestiona la sesion activa y coordina servicios de guardado, carga, slots, autosave y restauracion de escena.
 * Relaciones: Implementa ISaveCheckpointUseCase, IRestoreCheckpointUseCase, ISaveSlotReader, IGameSessionLoader e IGameDataProvider.
 * Riesgo arquitectonico mitigado: la logica real vive en SaveService/LoadService/SaveSlotService/SceneRestoreCoordinator/AutosaveController.
 * Uso como referencia: la UI y los interactables deben consumir interfaces por Inspector; GameManager.Instance queda limitado a debug/compatibilidad legacy.
 */
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour,
    ISaveCheckpointUseCase,
    IRestoreCheckpointUseCase,
    ISaveSlotReader,
    IGameSessionLoader,
    IGameDataProvider
{
    public static GameManager Instance { get; private set; }

    [Header("Auto Save")]
    [SerializeField] private float autoSaveInterval = 3600f;
    [SerializeField] private bool enableAutoSave = true;

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Decoupled Save")]
    [SerializeField] private SaveParticipantRegistry participantRegistry;
    [SerializeField] private ItemDatabase_SO itemDatabase;

    private SaveController saveController;
    private SaveService saveService;
    private LoadService loadService;
    private SaveSlotService saveSlotService;
    private SceneRestoreCoordinator sceneRestoreCoordinator;
    private AutosaveController autosaveController;

    private GameData currentGameData;
    private string currentSlotID = "1";
    private float sessionStartTime;
    private bool isInGame;

    public string CurrentSlotID => currentSlotID;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildServices();
        SaveParticipantRegistry.OnRegistryAvailable += HandleRegistryAvailable;
    }

    private void Start()
    {
        sessionStartTime = Time.time;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (!isInGame || !enableAutoSave || currentGameData == null)
            return;

        if (autosaveController != null && autosaveController.Tick(Time.deltaTime))
            AutoSave();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SaveParticipantRegistry.OnRegistryAvailable -= HandleRegistryAvailable;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentGameData == null)
            return;

        currentGameData.currentScene = scene.name;
        isInGame = scene.name != "Menu";

        if (isInGame)
            RestoreRuntimeData();
    }

    public void LoadGame(string slotID)
    {
        LoadGame(slotID, true);
    }

    public void LoadGame(string slotID, bool reloadScene)
    {
        GameData loadedData = loadService.LoadGame(slotID);

        if (loadedData == null)
        {
            Debug.LogError($"[GameManager] No se pudo cargar la partida del slot {slotID}");
            return;
        }

        currentGameData = loadedData;
        currentSlotID = slotID;
        sessionStartTime = Time.time;
        autosaveController.Reset();

        if (reloadScene)
        {
            SceneManager.LoadScene(currentGameData.currentScene);
            return;
        }

        RestoreRuntimeData();
    }

    public void CreateNewGame(string slotID)
    {
        currentGameData = GameData.CreateNewGame(slotID);
        currentSlotID = slotID;
        sessionStartTime = Time.time;
        autosaveController.Reset();
        isInGame = false;

        SceneManager.LoadScene(currentGameData.currentScene);
    }

    public void SaveGame()
    {
        TrySaveGame(false);
    }

    public bool TrySaveGame(bool createIfMissing)
    {
        if (currentGameData == null)
        {
            if (!createIfMissing)
            {
                Debug.LogWarning("[GameManager] No hay partida activa para guardar");
                return false;
            }

            CreateRuntimeGameDataForCurrentScene(currentSlotID);
        }

        UpdateRuntimeData();
        return saveService.SaveGame(currentGameData, currentSlotID);
    }

    public bool HasSaveFile(string slotID)
    {
        return saveSlotService.HasSaveFile(slotID);
    }

    public SaveInfo GetSaveInfo(string slotID)
    {
        return saveSlotService.GetSaveInfo(slotID);
    }

    public SaveInfo[] GetAllSaveInfos()
    {
        return saveSlotService.GetAllSaveInfos();
    }

    public void RefreshSaveStates()
    {
        Debug.Log("[GameManager] Save states refreshed");
    }

    public GameData GetCurrentGameData()
    {
        return currentGameData;
    }

    public string GetCurrentSlotID()
    {
        return currentSlotID;
    }

    public bool TrySaveCheckpoint(bool createIfMissing)
    {
        return TrySaveGame(createIfMissing);
    }

    public bool HasCheckpoint(string slotID)
    {
        return HasSaveFile(slotID);
    }

    public void RestoreCheckpoint(string slotID, bool reloadScene)
    {
        LoadGame(slotID, reloadScene);
    }

    public void UpdatePlayerPosition(Vector3 position)
    {
        if (currentGameData != null)
            currentGameData.playerData.SetPosition(position);
    }

    public void UpdatePlayerRotation(Quaternion rotation)
    {
        if (currentGameData != null)
            currentGameData.playerData.SetRotation(rotation);
    }

    public void UpdatePlayerHealth(int health, int maxHealth)
    {
        if (currentGameData == null)
            return;

        currentGameData.playerData.health = health;
        currentGameData.playerData.maxHealth = maxHealth;
    }

    public void RegisterScannedElement(string elementID)
    {
        if (currentGameData != null && !currentGameData.scannedElements.Contains(elementID))
            currentGameData.scannedElements.Add(elementID);
    }

    public void OpenOptions()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    public string DebugGetGameState()
    {
        if (currentGameData == null)
            return "Sin partida activa";

        var builder = new StringBuilder();
        builder.AppendLine("=== GAME STATE ===");
        builder.AppendLine($"Slot: {currentSlotID}");
        builder.AppendLine($"Escena: {currentGameData.currentScene}");
        builder.AppendLine($"Tiempo Jugado: {currentGameData.GetPlayTimeFormatted()}");
        builder.AppendLine($"Guardado: {currentGameData.lastSaveTime}");
        builder.AppendLine($"Items: {currentGameData.inventoryItems.Count}");
        builder.AppendLine($"Elementos Escaneados: {currentGameData.scannedElements.Count}");
        builder.AppendLine($"Posicion Jugador: {currentGameData.playerData.GetPosition()}");
        return builder.ToString();
    }

    private void AutoSave()
    {
        UpdateRuntimeData();
        saveService.SaveGame(currentGameData, currentSlotID);
    }

    private void BuildServices()
    {
        saveController = new SaveController();
        saveService = new SaveService(saveController);
        loadService = new LoadService(saveController);
        saveSlotService = new SaveSlotService(saveController);
        sceneRestoreCoordinator = new SceneRestoreCoordinator(participantRegistry, itemDatabase);
        autosaveController = new AutosaveController(autoSaveInterval);
    }

    private void CreateRuntimeGameDataForCurrentScene(string slotID)
    {
        var scene = SceneManager.GetActiveScene();

        currentSlotID = string.IsNullOrEmpty(slotID) ? "1" : slotID;
        currentGameData = GameData.CreateNewGame(currentSlotID);
        currentGameData.currentScene = scene.name;
        sessionStartTime = Time.time;
        autosaveController.Reset();
        isInGame = scene.name != "Menu";

        Debug.Log($"[GameManager] Partida runtime creada para guardar escena actual: {scene.name}");
    }

    private void RestoreRuntimeData()
    {
        if (currentGameData == null)
            return;

        SyncSceneCoordinator();
        sceneRestoreCoordinator.Restore(currentGameData);
    }

    private void UpdateRuntimeData()
    {
        // SaveLoad convierte estado runtime en DTOs serializables.
        // Consulta facades como InventoryController en lugar de guardar objetos Unity.
        if (currentGameData == null)
            return;

        currentGameData.UpdatePlayTime(Mathf.RoundToInt(Time.time - sessionStartTime));
        sessionStartTime = Time.time;

        SyncSceneCoordinator();
        sceneRestoreCoordinator.Capture(currentGameData);
    }

    private void SyncSceneCoordinator()
    {
        // Ruta oficial: SaveParticipantRegistry se asigna por Inspector o se anuncia al cargar la escena.
        sceneRestoreCoordinator.SetSceneDependencies(participantRegistry, itemDatabase);
    }

    private void HandleRegistryAvailable(SaveParticipantRegistry registry)
    {
        if (registry == null)
            return;

        participantRegistry = registry;

        if (registry.ItemDatabase != null)
            itemDatabase = registry.ItemDatabase;

        sceneRestoreCoordinator.SetSceneDependencies(participantRegistry, itemDatabase);
    }
}
