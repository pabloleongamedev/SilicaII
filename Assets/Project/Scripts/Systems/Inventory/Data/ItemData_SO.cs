using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/Item")]
public class ItemData_SO : ScriptableObject
{
    public string itemID;
    public string displayName;
    public Sprite icon;

    [Header("Stacking")]
    public int maxStack = 99;

    public int MaxStack => maxStack; // acceso consistente

    [TextArea]
    public string description;
}