using UnityEngine;

[CreateAssetMenu(menuName = "Chemistry/Database")]
public class SeparationDatabase_SO : ScriptableObject
{
    public CompoundDefinition_SO[] compounds;

    public CompoundDefinition_SO Get(ItemData_SO item)
    {
        if (item == null || compounds == null)
            return null;

        foreach (var c in compounds)
        {
            if (c == null || c.inputItem == null)
                continue;

            if (c.inputItem == item)
                return c;

            if (c.inputItem.itemID == item.itemID)
                return c;
        }

        return null;
    }
}
