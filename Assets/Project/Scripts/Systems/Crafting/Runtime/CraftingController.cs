/*
 * Arquitectura: Crafting/Runtime
 * Script: CraftingController
 * Rol: Facade runtime de Crafting. Coordina UI, CraftingSystem e Inventory mediante contratos.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Usa InventoryController solo para obtener IInventoryReadModel/IInventoryWriteModel y publica CraftingEventChannel_SO.
 * Riesgo arquitectonico mitigado: el router de escena decide si Quest/Notification reaccionan al crafting.
 * Uso como referencia: muestra como un Runtime puede orquestar Core + UI sin conocer consumidores externos.
 */
using UnityEngine;
using UnityEngine.UI;

public class CraftingController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private RecipeDatabase_SO database;

    [Header("Refs")]
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private ToolCraftingView toolView;
    [SerializeField] private CraftingRecipeListView listView;
    [SerializeField] private CraftingRecipeDetailView detailView;
    [SerializeField] private Button craftButton;

    [Header("Events")]
    [SerializeField] private CraftingEventChannel_SO craftingChannel;

    private CraftingSystem system;

    private IInventoryReadModel read;
    private IInventoryWriteModel write;

    private void Awake()
    {
        system = new CraftingSystem();
    }

    private void Start()
    {
        // Crafting colabora con Inventory mediante read/write models.
        // No accede a grids ni slots internos; pide operaciones al facade.
        if (inventoryController == null)
        {
            Debug.LogError("[CraftingController] InventoryController no asignado");
            return;
        }

        read = inventoryController.ReadModel;
        write = inventoryController.WriteModel;

        BuildRecipesUI();
        UpdateCraftButton();
    }

    private void OnEnable()
    {
        toolView.OnItemDroppedInSlot += HandleItemDropped;
        toolView.OnItemDragOut += HandleItemReturned;
        craftButton.onClick.AddListener(OnCraftClicked);
    }

    private void OnDisable()
    {
        toolView.OnItemDroppedInSlot -= HandleItemDropped;
        toolView.OnItemDragOut -= HandleItemReturned;
        craftButton.onClick.RemoveListener(OnCraftClicked);

        if (write != null)
        {
            system.ClearAll(write);
            toolView.Clear();
        }
    }

    private void BuildRecipesUI()
    {
        if (listView == null || database == null)
            return;

        listView.Build(database.recipes, OnRecipeSelected);
    }

    private void OnRecipeSelected(RecipeData_SO recipe)
    {
        // Al cambiar receta se devuelven items reservados para mantener
        // consistencia entre Core, Inventory y UI.
        system.ClearAll(write);

        system.SetRecipe(recipe);

        toolView.Clear();

        if (detailView != null)
            detailView.ShowRecipe(recipe);

        Notify("Receta seleccionada: " + recipe.name, NotificationType.Info);

        UpdateCraftButton();
    }

    private void HandleItemDropped(int slotIndex, ItemData_SO item)
    {
        var result = system.TryPlaceItem(slotIndex, item, read, write);

        if (!result.success)
        {
            Notify(result.message, ToNotificationType(result.type));
            return;
        }

        toolView.SetItemInSlot(slotIndex, item);

        Notify(result.message, ToNotificationType(result.type));

        UpdateCraftButton();
    }

    private void HandleItemReturned(int slotIndex, ItemData_SO item)
    {
        system.ClearSlot(slotIndex, write);
        toolView.ClearSlot(slotIndex);

        Notify($"{item.itemID} devuelto", NotificationType.Info);

        UpdateCraftButton();
    }

    private void UpdateCraftButton()
    {
        if (craftButton == null)
            return;

        craftButton.interactable = system.IsRecipeComplete();
    }

    private void OnCraftClicked()
    {
        // Flujo de crafting: validar Core, producir en Inventory, publicar evento
        // para Quest y limpiar la representacion visual.
        var recipe = system.GetCurrentRecipe();

        if (recipe == null)
        {
            Notify("No hay receta seleccionada", NotificationType.Warning);
            return;
        }

        if (!system.IsRecipeComplete())
        {
            Notify("Receta incompleta", NotificationType.Warning);

            system.ClearAll(write);
            toolView.Clear();

            UpdateCraftButton();
            return;
        }

        // Inventory procesa la produccion y publica sus propias notificaciones.
        var consumed = new (ItemData_SO item, int amount)[0];
        var produced = new[] { (recipe.result, recipe.resultAmount) };

        if (!write.TryProcessBatch(consumed, produced))
        {
            Notify("No hay espacio para el resultado", NotificationType.Warning);
            system.ClearAll(write);
            toolView.Clear();
            UpdateCraftButton();
            return;
        }
        craftingChannel?.RaiseItemCrafted(recipe.result, recipe.resultAmount);
        craftingChannel?.RaiseItemCraftedByID(recipe.result != null ? recipe.result.itemID : string.Empty, recipe.resultAmount);
        system.ClearAllNoReturn();

        toolView.Clear();

        Notify("Crafteo completado", NotificationType.Success);

        UpdateCraftButton();
    }

    private void Notify(string message, NotificationType type)
    {
        var notification = new NotificationData
        {
            message = message,
            type = type
        };

        craftingChannel?.RaiseNotification(notification);
    }

    private NotificationType ToNotificationType(CraftingResultType type)
    {
        // Runtime adapta resultados de dominio a Notification.
        switch (type)
        {
            case CraftingResultType.Success: return NotificationType.Success;
            case CraftingResultType.Error: return NotificationType.Error;
            case CraftingResultType.Info: return NotificationType.Info;
            default: return NotificationType.Warning;
        }
    }
}
