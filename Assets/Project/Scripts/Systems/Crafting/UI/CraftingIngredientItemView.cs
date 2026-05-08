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