/*
 * Arquitectura: Crafting/UI
 * Script: CraftingIngredientItemView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingIngredientItemView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;

    public void SetData(IngredientRequirement data)
    {
        icon.sprite = data.item.icon;
        nameText.text = data.item.displayName;
        amountText.text = $"x{data.amount}";
    }
}