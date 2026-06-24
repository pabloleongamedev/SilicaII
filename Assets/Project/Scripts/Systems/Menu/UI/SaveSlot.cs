/*
 * Arquitectura: Menu/UI
 * Script: SaveSlot
 * Rol: Vista/controlador de un slot de guardado simplificado.
 * Modulo: Gestiona el estado visual de Continuar/Nueva Partida.
 * Relaciones: Lee ISaveSlotReader y dispara IGameSessionLoader sobre el slot unico "1".
 * Riesgo arquitectonico mitigado: no conoce la implementacion concreta de SaveLoad; requiere asignar servicios por Inspector.
 * Uso como referencia: la vista conoce intenciones de aplicacion, no detalles de JSON, disco ni singleton.
 */
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    private enum SlotAction
    {
        Auto,
        NewGame,
        LoadGame
    }

    [Header("Configuracion")]
    [SerializeField] private SlotAction slotAction = SlotAction.Auto;
    [SerializeField] private TextMeshProUGUI textLabel;
    [SerializeField] private TextMeshProUGUI infoLabel;
    [SerializeField] private MonoBehaviour saveSlotReaderBehaviour;
    [SerializeField] private MonoBehaviour gameSessionLoaderBehaviour;

    [Header("Loading Transition")]
    [SerializeField] private GameObject loadingImage;
    [SerializeField, Min(0f)] private float minimumLoadingTime = 1f;
    [SerializeField, Min(0f)] private float fadeOutDuration = 0.35f;

    private const string UniqueSlot = "1";

    private bool hasData;
    private SaveInfo saveInfo;
    private ISaveSlotReader saveSlotReader;
    private IGameSessionLoader gameSessionLoader;
    private Button button;
    private Coroutine loadingRoutine;

    private void Awake()
    {
        button = GetComponent<Button>();
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

        hasData = saveSlotReader != null && saveSlotReader.HasSaveFile(UniqueSlot);
        saveInfo = saveSlotReader != null ? saveSlotReader.GetSaveInfo(UniqueSlot) : null;
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

        if (slotAction == SlotAction.NewGame)
        {
            Debug.Log("[SaveSlot] Creando nueva partida...");
            BeginLoadingThenRun(() => gameSessionLoader.CreateNewGame(UniqueSlot));
            return;
        }

        if (slotAction == SlotAction.LoadGame)
        {
            if (!hasData)
            {
                Debug.LogWarning("[SaveSlot] No hay partida guardada para cargar.", this);
                return;
            }

            Debug.Log("[SaveSlot] Cargando partida guardada...");
            BeginLoadingThenRun(() => gameSessionLoader.LoadGame(UniqueSlot));
            return;
        }

        if (hasData)
        {
            Debug.Log("[SaveSlot] Cargando partida guardada...");
            BeginLoadingThenRun(() => gameSessionLoader.LoadGame(UniqueSlot));
        }
        else
        {
            Debug.Log("[SaveSlot] Creando nueva partida...");
            BeginLoadingThenRun(() => gameSessionLoader.CreateNewGame(UniqueSlot));
        }
    }

    private void UpdateSlotVisual()
    {
        if (textLabel == null)
        {
            Debug.LogError("[SaveSlot] TextLabel no asignado", this);
            return;
        }

        if (slotAction == SlotAction.NewGame)
        {
            textLabel.text = "Nueva Partida";

            if (infoLabel != null)
                infoLabel.text = "Iniciar desde cero";

            if (button != null)
                button.interactable = true;

            return;
        }

        if (slotAction == SlotAction.LoadGame)
        {
            textLabel.text = "Cargar Partida";

            if (infoLabel != null)
                infoLabel.text = hasData && saveInfo != null
                    ? $"{saveInfo.playTime} | {saveInfo.lastSaveTime}"
                    : "Sin partida guardada";

            if (button != null)
                button.interactable = hasData;

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

    private void BeginLoadingThenRun(System.Action loadAction)
    {
        if (loadingRoutine != null)
            return;

        if (button != null)
            button.interactable = false;

        loadingRoutine = StartCoroutine(LoadingRoutine(loadAction));
    }

    private IEnumerator LoadingRoutine(System.Action loadAction)
    {
        yield return FadeOutLoadingImage();

        float remainingDelay = Mathf.Max(0f, minimumLoadingTime - fadeOutDuration);
        if (remainingDelay > 0f)
            yield return new WaitForSeconds(remainingDelay);

        loadAction?.Invoke();
    }

    private IEnumerator FadeOutLoadingImage()
    {
        if (loadingImage == null)
            yield break;

        var loadingGraphic = loadingImage.GetComponent<Graphic>();
        loadingImage.SetActive(true);

        if (loadingGraphic == null)
            yield break;

        Color targetColor = loadingGraphic.color;
        Color startColor = targetColor;
        startColor.a = 0f;
        loadingGraphic.color = startColor;

        if (fadeOutDuration <= 0f)
        {
            loadingGraphic.color = targetColor;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeOutDuration);
            loadingGraphic.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        loadingGraphic.color = targetColor;
    }
}
