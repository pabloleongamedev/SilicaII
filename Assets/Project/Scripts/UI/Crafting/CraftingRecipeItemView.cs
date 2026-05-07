using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class CraftingRecipeItemView : MonoBehaviour
{
    //[SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;

    private RecipeData_SO recipe;

    public Action<RecipeData_SO> OnClicked;

    public void SetData(RecipeData_SO recipe)
    {
        this.recipe = recipe;

        //icon.sprite = recipe.result.icon;
        nameText.text = recipe.name;
    }

    public void OnClick()
    {
        OnClicked?.Invoke(recipe);
    }
}