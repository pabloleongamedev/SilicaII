/*
 * Arquitectura: SaveLoad/Runtime
 * Script: SaveLoadSceneBinding
 * Rol: Fachada principal de SaveLoad por escena.
 * Relaciones: Checkpoints, menu y UI consumen sus interfaces; SaveParticipantRegistry captura/restaura el estado modular de Player, Inventory, Timer y Jetpack.
 * Riesgo arquitectonico mitigado: elimina la fachada global anterior; la sesion, servicios, slots, autosave y restauracion quedan visibles en escena.
 * Uso como referencia: cada escena que pueda guardar/cargar debe tener este binding y asignar registry, database y scene loader por Inspector.
 */
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadSceneBinding : MonoBehaviour,
    ISaveCheckpointUseCase,
    IRestoreCheckpointUseCase,
    ISaveSlotReader,
    IGameSessionLoader,
    IGameDataProvider
{
    private static GameData pendingSceneGameData;
    private static string pendingSceneSlotID;

    [Header("Auto Save")]
    [SerializeField] private float autoSaveInterval = 3600f;
    [SerializeField] private bool enableAutoSave = true;

    [Header("Scene Dependencies")]
    [SerializeField] private SaveParticipantRegistry participantRegistry;
    [SerializeField] private ItemDatabase_SO itemDatabase;

    [Header("Scene Loading")]
    [SerializeField] private MonoBehaviour sceneLoaderBehaviour;

    private SaveController saveController;
    private SaveService saveService;
    private LoadService loadService;
    private SaveSlotService saveSlotService;
    private SceneRestoreCoordinator sceneRestoreCoordinator;
    private AutosaveController autosaveController;
    private ISceneLoader sceneLoader;

    private GameData currentGameData;
    private string currentSlotID = "1";
    private float sessionStartTime;
    private bool isInGame;

    public string CurrentSlotID => currentSlotID;

    private void Awake()
    {
        ResolveSceneLoader();
        BuildServices();
        ConsumePendingSceneSession();
        SyncSceneCoordinator();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SyncSceneCoordinator();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (!isInGame || !enableAutoSave || currentGameData == null)
            return;

        if (autosaveController != null && autosaveController.Tick(Time.deltaTime))
            AutoSave();
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
            Debug.LogError($"[SaveLoadSceneBinding] No se pudo cargar la partida del slot {slotID}", this);
            return;
        }

        currentGameData = loadedData;
        currentSlotID = slotID;
        sessionStartTime = Time.time;
        autosaveController.Reset();

        if (reloadScene)
        {
            PreparePendingSceneSession();
            ResolveSceneLoader().LoadScene(currentGameData.currentScene);
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

        PreparePendingSceneSession();
        ResolveSceneLoader().LoadScene(currentGameData.currentScene);
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

    public GameData GetCurrentGameData()
    {
        return currentGameData;
    }

    public string GetCurrentSlotID()
    {
        return currentSlotID;
    }

    public void ConfigureSceneSaveDependencies(SaveParticipantRegistry registry, ItemDatabase_SO database)
    {
        participantRegistry = registry;
        itemDatabase = database != null ? database : registry != null ? registry.ItemDatabase : itemDatabase;
        SyncSceneCoordinator();
    }

    public bool TrySaveGame(bool createIfMissing)
    {
        if (currentGameData == null)
        {
            if (!createIfMissing)
            {
                Debug.LogWarning("[SaveLoadSceneBinding] No hay partida activa para guardar.", this);
                return false;
            }

            CreateRuntimeGameDataForCurrentScene(currentSlotID);
        }

        UpdateRuntimeData();
        return saveService.SaveGame(currentGameData, currentSlotID);
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentGameData == null)
            return;

        currentGameData.currentScene = scene.name;
        isInGame = scene.name != "Menu";

        if (isInGame)
            RestoreRuntimeData();
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

        Debug.Log($"[SaveLoadSceneBinding] Partida runtime creada para guardar escena actual: {scene.name}", this);
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
        if (currentGameData == null)
            return;

        currentGameData.UpdatePlayTime(Mathf.RoundToInt(Time.time - sessionStartTime));
        sessionStartTime = Time.time;

        SyncSceneCoordinator();
        sceneRestoreCoordinator.Capture(currentGameData);
    }

    private void SyncSceneCoordinator()
    {
        if (sceneRestoreCoordinator == null)
            return;

        itemDatabase = itemDatabase != null ? itemDatabase : participantRegistry != null ? participantRegistry.ItemDatabase : null;
        sceneRestoreCoordinator.SetSceneDependencies(participantRegistry, itemDatabase);
    }

    private void PreparePendingSceneSession()
    {
        // Transferencia entre escenas: evita una fachada DontDestroyOnLoad y permite
        // que cada escena siga declarando su propio SaveLoadSceneBinding.
        pendingSceneGameData = currentGameData;
        pendingSceneSlotID = currentSlotID;
    }

    private void ConsumePendingSceneSession()
    {
        if (pendingSceneGameData == null)
            return;

        currentGameData = pendingSceneGameData;
        currentSlotID = string.IsNullOrEmpty(pendingSceneSlotID) ? "1" : pendingSceneSlotID;
        sessionStartTime = Time.time;
        isInGame = SceneManager.GetActiveScene().name != "Menu";
        autosaveController.Reset();

        pendingSceneGameData = null;
        pendingSceneSlotID = null;

        if (isInGame)
            RestoreRuntimeData();
    }

    private ISceneLoader ResolveSceneLoader()
    {
        if (sceneLoader == null)
            sceneLoader = sceneLoaderBehaviour as ISceneLoader;

        if (sceneLoader == null && sceneLoaderBehaviour != null)
            Debug.LogWarning("[SaveLoadSceneBinding] El Scene Loader asignado no implementa ISceneLoader.", this);

        if (sceneLoader == null)
            sceneLoader = new UnitySceneLoader();

        return sceneLoader;
    }
}
