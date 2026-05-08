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

    private SeparationMethod_SO currentMethod;
    private CompoundDefinition_SO currentCompound;

    private IInventoryReadModel read;
    private IInventoryWriteModel write;

    private ChemistrySystem system;

    private bool isCleaning;
    private bool isProcessing;

    // =========================================================
    private void Awake()
    {
        system = new ChemistrySystem();
    }

    private void Start()
    {
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
        GameplayEvents.OnUIStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        GameplayEvents.OnUIStateChanged -= HandleStateChanged;
    }

    private void OnDestroy()
    {
        inventoryListView.OnItemDropped -= HandleInventoryDrop;
        toolView.OnItemPlaced -= HandleItemPlaced;
        toolView.OnItemCleared -= HandleItemCleared;
    }

    // =========================================================
    // PANEL CLOSE → CLEANUP
    // =========================================================
    private void HandleStateChanged(UIState state)
    {
        if (state != UIState.Chemistry)
        {
            Cleanup();
        }
    }

    private void Cleanup()
    {
        if (currentCompound == null && toolView.GetItem() == null)
            return;

        isCleaning = true;

        // 🔥 rollback SOLO si había item
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
            Notify("Slot vacío", NotificationType.Warning);
            return;
        }

        var item = itemInstance.Data;

        // =====================================================
        // 🔥 VALIDAR ANTES DE SETEAR
        // =====================================================
        var compound = database.Get(item);

        if (compound == null)
        {
            Notify("Este elemento no se puede separar", NotificationType.Warning);
            return; // ❌ NO entra al slot
        }

        if (toolView.GetItem() != null)
        {
            Notify("El refinador ya está ocupado", NotificationType.Warning);
            return;
        }

        // =====================================================
        // ✔ SOLO SI PASA TODO → DROP REAL
        // =====================================================
        toolView.SetItem(item);
    }
    // =========================================================
    // 🔥 DROP (CONSUMO REAL AQUÍ)
    // =========================================================
    private void HandleItemPlaced(ItemData_SO item)
    {

        var compound = database.Get(item);

        if (compound == null)
        {
            Notify("Este item no se puede refinar", NotificationType.Warning);
            return;
        }

        //  VALIDAR ANTES
        int before = read.GetAmount(item);

        if (before <= 0)
        {
            Notify("No hay item en inventario", NotificationType.Warning);
            return;
        }

        //  REMOVER (void)
        write.RemoveItem(item, 1);

        //  VALIDAR DESPUÉS (debug crítico)
        int after = read.GetAmount(item);

        if (after == before)
        {
            return;
        }

        currentCompound = compound;

        Notify($"{item.itemID} listo para refinar", NotificationType.Info);

        UpdateButton();
    }

    // =========================================================
    // 🔴 CLEAR (ROLLBACK)
    // =========================================================
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

    // =========================================================
    // METHOD
    // =========================================================
    private void OnMethodSelected(SeparationMethod_SO method)
    {
        currentMethod = method;

        if (descriptionText != null)
            descriptionText.text = method.description;

        Notify("Método seleccionado: " + method.methodName, NotificationType.Info);

        UpdateButton();
    }

    private void UpdateButton()
    {
        if (refineButton == null) return;

        // SIEMPRE ACTIVO
        refineButton.interactable = true;
    }

    // =========================================================
    // EXECUTE
    // =========================================================
    private void OnRefineClicked()
    {

        if (currentCompound == null)
        {
            Notify("Debes colocar un compuesto en el refinador", NotificationType.Warning);
            return;
        }

        if (currentMethod == null)
        {
            Notify("Debes seleccionar un método de separación", NotificationType.Warning);
            return;
        }

        if (currentCompound.requiredMethod != currentMethod)
        {
            Notify("Método incorrecto para este compuesto", NotificationType.Error);
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

        // 🔥 MISIONES
        CraftingEvents.OnItemRefined?.Invoke(currentCompound.inputItem, 1);
        QuestEvents.OnItemRefined?.Invoke(currentCompound.inputItem, 1);

        // 🔥 limpiar sin rollback
        isCleaning = true;
        toolView.Clear();
        isCleaning = false;

        currentCompound = null;

        Notify("Elemento refinado con éxito", NotificationType.Success);

        UpdateButton();
    }

    // =========================================================
    private void Notify(string msg, NotificationType type)
    {
        var notification = new NotificationData
        {
            message = msg,
            type = type
        };

        CraftingEvents.OnNotificationRequested?.Invoke(notification);
        GameplayEvents.OnNotification?.Invoke(notification);
    }
}
