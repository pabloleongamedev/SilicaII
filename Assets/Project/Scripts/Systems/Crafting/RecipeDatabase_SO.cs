using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe Database")]
public class RecipeDatabase_SO : ScriptableObject
{
    public List<RecipeData_SO> recipes;
}