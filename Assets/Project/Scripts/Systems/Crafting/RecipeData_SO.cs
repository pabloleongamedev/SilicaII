using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class RecipeData_SO : ScriptableObject
{
    [Header("Resultado")]
    public ItemData_SO result;
    public int resultAmount = 1;

    [Header("Ingredientes")]
    public List<IngredientRequirement> ingredients;

    [TextArea]
    public string description;
}