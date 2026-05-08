using UnityEngine;

[CreateAssetMenu(menuName = "Data/Element")]
public class ElementData : ScriptableObject
{
    public string elementName;
    public string symbol;
    public int rarity;
}