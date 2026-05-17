/*
 * Arquitectura: Scanner/Data
 * Script: ScanDefinition_SO
 * Rol: Dato editable propio del sistema Scanner para describir resultados detectables.
 * Relaciones: ScannableObject consume este asset y publica sus outputs por NotificationEvents sin depender de Crafting.
 * Riesgo arquitectonico mitigado: Scanner deja de depender de CompoundDefinition_SO, que pertenece a Chemistry/Crafting.
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Scanner/Scan Definition")]
public class ScanDefinition_SO : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private List<ScanOutput> outputs = new List<ScanOutput>();

    public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
    public IReadOnlyList<ScanOutput> Outputs => outputs;

    [System.Serializable]
    public struct ScanOutput
    {
        public ItemData_SO item;
        public int amount;
    }
}
