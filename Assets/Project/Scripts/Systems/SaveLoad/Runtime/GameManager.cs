/*
 * Arquitectura: SaveLoad/Runtime
 * Script: GameManager
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona datos de partida, guardado/carga y restauracion de estado runtime.
 * Relaciones: Consulta facades runtime como InventoryController y PlayerInputHandler para persistir/restaurar datos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Auto Save")]
    [SerializeField] private float autoSaveInterval = 60f;
    [SerializeField] private bool enableAutoSave = true;

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    private SaveController saveController;
    private GameData currentGameData;
    private string currentSlotID = "1";
    private float timeSinceLastSave;
    private float sessionStartTime;
    private bool isInGame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        saveController = new SaveController();
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

        timeSinceLastSave += Time.deltaTime;

        if (timeSinceLastSave >= autoSaveInterval)
        {
            AutoSave();
            timeSinceLastSave = 0f;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentGameData == null)
            return;

        currentGameData.currentScene = scene.name;
        isInGame = scene.name != "Menu";
    }

    public void LoadGame(string slotID)
    {
        GameData loadedData = saveController.LoadGame(slotID);

        if (loadedData == null)
        {
            Debug.LogError($"[GameManager] No se pudo cargar la partida del slot {slotID}");
            return;
        }

        currentGameData = loadedData;
        currentSlotID = slotID;
        sessionStartTime = Time.time;
        timeSinceLastSave = 0f;

        SceneManager.LoadScene(currentGameData.currentScene);
    }

    public void CreateNewGame(string slotID)
    {
        currentGameData = GameData.CreateNewGame(slotID);
        currentSlotID = slotID;
        sessionStartTime = Time.time;
        timeSinceLastSave = 0f;
        isInGame = false;

        SceneManager.LoadScene(currentGameData.currentScene);
    }

    public void SaveGame()
    {
        if (currentGameData == null)
        {
            Debug.LogWarning("[GameManager] No hay partida activa para guardar");
            return;
        }

        UpdateRuntimeData();
        saveController.SaveGame(currentGameData, currentSlotID);
    }

    public bool HasSaveFile(string slotID)
    {
        return saveController.HasSaveFile(slotID);
    }

    public SaveInfo GetSaveInfo(string slotID)
    {
        return saveController.GetSaveInfo(slotID);
    }

    public SaveInfo[] GetAllSaveInfos()
    {
        return saveController.GetAllSaveInfos();
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
        saveController.SaveGame(currentGameData, currentSlotID);
    }

    private void UpdateRuntimeData()
    {
        // SaveLoad convierte estado runtime en DTOs serializables.
        // Consulta facades como InventoryController en lugar de guardar objetos Unity.
        if (currentGameData == null)
            return;

        currentGameData.UpdatePlayTime(Mathf.RoundToInt(Time.time - sessionStartTime));
        sessionStartTime = Time.time;

        var playerInput = FindFirstObjectByType<PlayerInputHandler>();
        if (playerInput != null)
        {
            currentGameData.playerData.SetPosition(playerInput.transform.position);
            currentGameData.playerData.SetRotation(playerInput.transform.rotation);
        }

        var inventory = FindFirstObjectByType<InventoryController>();
        if (inventory != null)
            currentGameData.inventoryItems = inventory.ExportSaveData();
    }
}
