/*
 * Arquitectura: Crafting/UI
 * Script: CraftingRecipeListView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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