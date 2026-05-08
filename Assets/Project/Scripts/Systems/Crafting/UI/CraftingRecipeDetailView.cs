/*
 * Arquitectura: Crafting/UI
 * Script: CraftingRecipeDetailView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using TMPro;

public class CraftingRecipeDetailView : MonoBehaviour
{
    [SerializeField] private Transform ingredientContainer;
    [SerializeField] private CraftingIngredientItemView ingredientPrefab;

    [SerializeField] private TextMeshProUGUI descriptionText;

    public void ShowRecipe(RecipeData_SO recipe)
    {
        // limpiar
        foreach (Transform child in ingredientContainer)
            Destroy(child.gameObject);

        // ingredientes
        foreach (var ing in recipe.ingredients)
        {
            var item = Instantiate(ingredientPrefab, ingredientContainer);
            item.SetData(ing);
        }

        // descripción
        descriptionText.text = recipe.description;
    }
}