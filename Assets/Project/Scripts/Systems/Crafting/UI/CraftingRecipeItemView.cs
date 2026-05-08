/*
 * Arquitectura: Crafting/UI
 * Script: CraftingRecipeItemView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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