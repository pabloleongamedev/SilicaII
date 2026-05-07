using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipeListView : MonoBehaviour
{
    [SerializeField] private CraftingRecipeItemView itemPrefab;
    [SerializeField] private Transform container;

    private List<CraftingRecipeItemView> items = new();

    public void Build(List<RecipeData_SO> recipes, System.Action<RecipeData_SO> onClick)
    {
        Debug.Log("RECIPES COUNT: " + recipes.Count);
        
        foreach (Transform child in container)
            Destroy(child.gameObject);

        items.Clear();

        foreach (var recipe in recipes)
        {
            var item = Instantiate(itemPrefab, container);

            item.SetData(recipe);
            item.OnClicked += onClick;

            items.Add(item);
        }
    }
}