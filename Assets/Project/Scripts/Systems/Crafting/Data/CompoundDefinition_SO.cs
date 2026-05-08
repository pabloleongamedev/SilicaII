using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chemistry/Compound")]
public class CompoundDefinition_SO : ScriptableObject
{
    public ItemData_SO inputItem; //  EL ITEM QUE SE SEPARA

    public SeparationMethod_SO requiredMethod; // MÉTODO CORRECTO

    [System.Serializable]
    public struct OutputElement
    {
        public ItemData_SO item;
        public int amount;
    }

    public List<OutputElement> outputs;
}