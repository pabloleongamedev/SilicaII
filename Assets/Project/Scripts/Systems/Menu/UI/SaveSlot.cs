/*
 * Arquitectura: Menu/UI
 * Script: SaveSlot
 * Rol: Vista/controlador de un slot de guardado simplificado.
 * Modulo: Gestiona el estado visual de Continuar/Nueva Partida.
 * Relaciones: Lee ISaveSlotReader y dispara IGameSessionLoader sobre el slot unico "1".
 * Riesgo arquitectonico mitigado: no conoce GameManager; requiere asignar servicios por Inspector.
 * Uso como referencia: la vista conoce intenciones de aplicacion, no detalles de JSON, disco ni singleton.
 */
using TMPro;
using UnityEngine;

public class SaveSlot : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private TextMeshProUGUI textLabel;
    [SerializeField] private TextMeshProUGUI infoLabel;
    [SerializeField] private MonoBehaviour saveSlotReaderBehaviour;
    [SerializeField] private MonoBehaviour gameSessionLoaderBehaviour;

    private const string UniqueSlot = "1";

    private bool hasData;
    private SaveInfo saveInfo;
    private ISaveSlotReader saveSlotReader;
    private IGameSessionLoader gameSessionLoader;

    private void Awake()
    {
        saveSlotReader = saveSlotReaderBehaviour as ISaveSlotReader;
        gameSessionLoader = gameSessionLoaderBehaviour as IGameSessionLoader;
    }

    private void Start()
    {
        ResolveSerializedServices();
    }

    public void RefreshSlot()
    {
        ResolveSerializedServices();

        if (saveSlotReader == null)
            return;

        hasData = saveSlotReader.HasSaveFile(UniqueSlot);
        saveInfo = saveSlotReader.GetSaveInfo(UniqueSlot);
        UpdateSlotVisual();
    }

    public void OnSlotPressed()
    {
        ResolveSerializedServices();

        if (gameSessionLoader == null)
        {
            Debug.LogWarning("[SaveSlot] No existe IGameSessionLoader para cargar o crear partida.", this);
            return;
        }

        if (hasData)
        {
            Debug.Log("[SaveSlot] Cargando partida guardada...");
            gameSessionLoader.LoadGame(UniqueSlot);
        }
        else
        {
            Debug.Log("[SaveSlot] Creando nueva partida...");
            gameSessionLoader.CreateNewGame(UniqueSlot);
        }
    }

    private void UpdateSlotVisual()
    {
        if (textLabel == null)
        {
            Debug.LogError("[SaveSlot] TextLabel no asignado", this);
            return;
        }

        if (hasData && saveInfo != null)
        {
            textLabel.text = "Continuar";

            if (infoLabel != null)
                infoLabel.text = $"{saveInfo.playTime} | {saveInfo.lastSaveTime}";

            return;
        }

        textLabel.text = "Nueva Partida";

        if (infoLabel != null)
            infoLabel.text = "Sin partida guardada";
    }

    private void ResolveSerializedServices()
    {
        if (saveSlotReader == null)
            saveSlotReader = saveSlotReaderBehaviour as ISaveSlotReader;

        if (gameSessionLoader == null)
            gameSessionLoader = gameSessionLoaderBehaviour as IGameSessionLoader;
    }
}
