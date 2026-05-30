/*
 * Arquitectura: Crafting/Runtime
 * Script: ChemistryController
 * Rol: Facade runtime de Chemistry. Coordina UI, ChemistrySystem, metodos de separacion e Inventory.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Usa IInventoryReadModel/IInventoryWriteModel para transacciones; escucha UIStateEventChannel_SO y publica CraftingEventChannel_SO.
 * Riesgo arquitectonico mitigado: el router de escena decide si Quest/Notification reaccionan a Chemistry.
 * Uso como referencia: documenta un controlador runtime funcional que aun puede adelgazar separando flujo UI y transacciones.
 */
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChemistryController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SeparationMethod_SO[] methods;
    [SerializeField] private SeparationDatabase_SO database;

    [Header("UI")]
    [SerializeField] private MethodListView methodListView;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private ToolChemistryView toolView;
    [SerializeField] private InventoryListView inventoryListView;
    [SerializeField] private Button refineButton;

    [Header("Refs")]
    [SerializeField] private InventoryController inventoryController;

    [Header("Events")]
    [SerializeField] private UIStateEventChannel_SO uiStateChannel;
    [SerializeField] private CraftingEventChannel_SO craftingChannel;

    private SeparationMethod_SO currentMethod;
    private CompoundDefinition_SO currentCompound;

    private IInventoryReadModel read;
    private IInventoryWriteModel write;

    private ChemistrySystem system;

    private bool isCleaning;
    private bool isProcessing;

    private void Awake()
    {
        system = new ChemistrySystem();
    }

    private void Start()
    {
        // Chemistry usa Inventory como servicio externo: lee disponibilidad y
        // solicita transacciones, pero no manipula slots internos.
        read = inventoryController.ReadModel;
        write = inventoryController.WriteModel;

        inventoryListView.Initialize(read);
        inventoryListView.OnItemDropped += HandleInventoryDrop;

        methodListView.Build(methods, OnMethodSelected);

        toolView.OnItemPlaced += HandleItemPlaced;
        toolView.OnItemCleared += HandleItemCleared;

        refineButton.onClick.AddListener(OnRefineClicked);

        UpdateButton();
    }

    private void OnEnable()
    {
        if (uiStateChannel != null)
            uiStateChannel.Raised += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (uiStateChannel != null)
            uiStateChannel.Raised -= HandleStateChanged;
    }

    private void OnDestroy()
    {
        inventoryListView.OnItemDropped -= HandleInventoryDrop;
        toolView.OnItemPlaced -= HandleItemPlaced;
        toolView.OnItemCleared -= HandleItemCleared;
    }

    private void HandleStateChanged(UIState state)
    {
        if (state != UIState.Chemistry)
        {
            Cleanup();
        }
    }

    private void Cleanup()
    {
        // Al cerrar el panel se devuelve el item reservado, evitando perdida de
        // inventario si el jugador abandona el flujo antes de refinar.
        if (currentCompound == null && toolView.GetItem() == null)
            return;

        isCleaning = true;

        if (currentCompound != null)
        {
            write.AddItem(currentCompound.inputItem, 1);
        }

        toolView.Clear();

        currentCompound = null;
        currentMethod = null;

        isCleaning = false;

        Notify("Refinador limpiado", NotificationType.Info);
        UpdateButton();
    }

    private void HandleInventoryDrop(int fromIndex, int toIndex)
    {
        var itemInstance = read.GetItem(fromIndex);

        if (itemInstance == null)
        {
            Notify("Slot vacio", NotificationType.Warning);
            return;
        }

        var item = itemInstance.Data;

        var compound = database.Get(item);

        if (compound == null)
        {
            Notify("Este elemento no se puede separar", NotificationType.Warning);
            return;
        }

        if (toolView.GetItem() != null)
        {
            Notify("El refinador ya esta ocupado", NotificationType.Warning);
            return;
        }

        toolView.SetItem(item);
    }

    private void HandleItemPlaced(ItemData_SO item)
    {

        var compound = database.Get(item);

        if (compound == null)
        {
            Notify("Este item no se puede refinar", NotificationType.Warning);
            return;
        }

        int before = read.GetAmount(item);

        if (before <= 0)
        {
            Notify("No hay item en inventario", NotificationType.Warning);
            return;
        }

        write.RemoveItem(item, 1);

        int after = read.GetAmount(item);

        if (after == before)
        {
            return;
        }

        currentCompound = compound;

        Notify($"{item.itemID} listo para refinar", NotificationType.Info);

        UpdateButton();
    }

    private void HandleItemCleared()
    {
        if (isCleaning || isProcessing)
            return;

        if (currentCompound == null)
            return;

        write.AddItem(currentCompound.inputItem, 1);

        currentCompound = null;

        Notify("Elemento devuelto al inventario", NotificationType.Info);

        UpdateButton();
    }

    private void OnMethodSelected(SeparationMethod_SO method)
    {
        currentMethod = method;

        if (descriptionText != null)
            descriptionText.text = method.description;

        Notify("Metodo seleccionado: " + method.methodName, NotificationType.Info);

        UpdateButton();
    }

    private void UpdateButton()
    {
        if (refineButton == null) return;

        refineButton.interactable = true;
    }

    private void OnRefineClicked()
    {
        // Runtime orquesta UI + Core + Inventory + Quest:
        // ChemistrySystem valida, Inventory aplica outputs y Quest recibe evento.

        if (currentCompound == null)
        {
            Notify("Debes colocar un compuesto en el refinador", NotificationType.Warning);
            return;
        }

        if (currentMethod == null)
        {
            Notify("Debes seleccionar un metodo de separacion", NotificationType.Warning);
            return;
        }

        if (currentCompound.requiredMethod != currentMethod)
        {
            Notify("Metodo incorrecto para este compuesto", NotificationType.Error);
            return;
        }

        if (!system.CanSeparate(currentCompound, currentMethod, read))
        {
            Notify("No hay espacio en el inventario", NotificationType.Warning);
            return;
        }

        isProcessing = true;

        bool success = system.Execute(
            currentCompound,
            currentMethod,
            read,
            write
        );

        isProcessing = false;

        if (!success)
        {
            Notify("No se pudo refinar", NotificationType.Error);
            return;
        }

        craftingChannel?.RaiseItemRefined(currentCompound.inputItem, 1);
        craftingChannel?.RaiseItemRefinedByID(currentCompound.inputItem != null ? currentCompound.inputItem.itemID : string.Empty, 1);

        // Limpia la vista despues de producir outputs; no devuelve input reservado.
        isCleaning = true;
        toolView.Clear();
        isCleaning = false;

        currentCompound = null;

        Notify("Elemento refinado con exito", NotificationType.Success);

        UpdateButton();
    }

    private void Notify(string msg, NotificationType type)
    {
        var notification = new NotificationData
        {
            message = msg,
            type = type
        };

        craftingChannel?.RaiseNotification(notification);
    }
}
