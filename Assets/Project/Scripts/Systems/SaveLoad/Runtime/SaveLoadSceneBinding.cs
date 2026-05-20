/*
 * Arquitectura: SaveLoad/Runtime
 * Script: SaveLoadSceneBinding
 * Rol: Binding explicito de escena para exponer casos de uso de guardado/carga por Inspector.
 * Relaciones: CheckpointSavePoint y CheckpointRestorePoint consumen este componente o un MonoBehaviour asignado que implemente las interfaces.
 * Riesgo arquitectonico mitigado: reemplaza SaveLoadUseCaseRegistry estatico; las dependencias quedan visibles en la jerarquia.
 * Uso como referencia: si luego GameManager desaparece, este binding puede delegar a otro orquestador sin tocar checkpoints o UI.
 */
using UnityEngine;

public class SaveLoadSceneBinding : MonoBehaviour,
    ISaveCheckpointUseCase,
    IRestoreCheckpointUseCase,
    ISaveSlotReader,
    IGameSessionLoader,
    IGameDataProvider
{
    [Header("Use Case Provider")]
    [SerializeField] private MonoBehaviour saveLoadProviderBehaviour;
    [SerializeField] private SaveParticipantRegistry participantRegistry;
    [SerializeField] private ItemDatabase_SO itemDatabase;

    private ISaveCheckpointUseCase saveCheckpointUseCase;
    private IRestoreCheckpointUseCase restoreCheckpointUseCase;
    private ISaveSlotReader saveSlotReader;
    private IGameSessionLoader gameSessionLoader;
    private IGameDataProvider gameDataProvider;

    public string CurrentSlotID => saveCheckpointUseCase?.CurrentSlotID
        ?? restoreCheckpointUseCase?.CurrentSlotID
        ?? "1";

    private void Awake()
    {
        ResolveProvider();
        ApplySceneSaveDependencies();
    }

    private void OnEnable()
    {
        ResolveProvider();
        ApplySceneSaveDependencies();
    }

    public bool TrySaveCheckpoint(bool createIfMissing)
    {
        ResolveProvider();
        return saveCheckpointUseCase != null && saveCheckpointUseCase.TrySaveCheckpoint(createIfMissing);
    }

    public bool HasCheckpoint(string slotID)
    {
        ResolveProvider();
        return restoreCheckpointUseCase != null && restoreCheckpointUseCase.HasCheckpoint(slotID);
    }

    public void RestoreCheckpoint(string slotID, bool reloadScene)
    {
        ResolveProvider();
        restoreCheckpointUseCase?.RestoreCheckpoint(slotID, reloadScene);
    }

    public bool HasSaveFile(string slotID)
    {
        ResolveProvider();
        return saveSlotReader != null && saveSlotReader.HasSaveFile(slotID);
    }

    public SaveInfo GetSaveInfo(string slotID)
    {
        ResolveProvider();
        return saveSlotReader?.GetSaveInfo(slotID);
    }

    public SaveInfo[] GetAllSaveInfos()
    {
        ResolveProvider();
        return saveSlotReader != null ? saveSlotReader.GetAllSaveInfos() : new SaveInfo[0];
    }

    public void LoadGame(string slotID)
    {
        ResolveProvider();
        gameSessionLoader?.LoadGame(slotID);
    }

    public void CreateNewGame(string slotID)
    {
        ResolveProvider();
        gameSessionLoader?.CreateNewGame(slotID);
    }

    public GameData GetCurrentGameData()
    {
        ResolveProvider();
        return gameDataProvider?.GetCurrentGameData();
    }

    public string GetCurrentSlotID()
    {
        ResolveProvider();
        return CurrentSlotID;
    }

    private void ResolveProvider()
    {
        saveCheckpointUseCase = saveLoadProviderBehaviour as ISaveCheckpointUseCase;
        restoreCheckpointUseCase = saveLoadProviderBehaviour as IRestoreCheckpointUseCase;
        saveSlotReader = saveLoadProviderBehaviour as ISaveSlotReader;
        gameSessionLoader = saveLoadProviderBehaviour as IGameSessionLoader;
        gameDataProvider = saveLoadProviderBehaviour as IGameDataProvider;
    }

    private void ApplySceneSaveDependencies()
    {
        if (saveLoadProviderBehaviour is GameManager gameManager)
            gameManager.ConfigureSceneSaveDependencies(participantRegistry, itemDatabase);
    }
}
