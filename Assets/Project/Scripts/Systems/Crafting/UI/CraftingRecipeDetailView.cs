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