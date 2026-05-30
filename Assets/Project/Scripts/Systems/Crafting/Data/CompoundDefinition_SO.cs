/*
 * Arquitectura: Crafting/Data
 * Script: CompoundDefinition_SO
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chemistry/Compound")]
public class CompoundDefinition_SO : ScriptableObject
{
    public ItemData_SO inputItem;

    public SeparationMethod_SO requiredMethod;

    [System.Serializable]
    public struct OutputElement
    {
        public ItemData_SO item;
        public int amount;
    }

    public List<OutputElement> outputs;
}
