/*
 * Arquitectura: Crafting/Core
 * Script: ChemistrySystem
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System.Linq;
using UnityEngine;

/// <summary>
/// Sistema PURO de lógica de separación.
/// ✔ No depende de MonoBehaviour
/// ✔ No depende de UI
/// ✔ No consume input (eso ya ocurre en el DROP)
/// ✔ Solo valida outputs y los ejecuta
/// </summary>
public class ChemistrySystem
{
    // =========================================================
    // VALIDACIÓN
    // =========================================================
    public bool CanSeparate(
        CompoundDefinition_SO compound,
        SeparationMethod_SO method,
        IInventoryReadModel read)
    {
        // 🔥 VALIDACIONES BASE
        if (compound == null)
        {
            Debug.LogWarning("CanSeparate: compound NULL");
            return false;
        }

        if (method == null)
        {
            Debug.LogWarning("CanSeparate: method NULL");
            return false;
        }

        // 🔥 VALIDAR MÉTODO
        if (compound.requiredMethod != method)
        {
            Debug.LogWarning("CanSeparate: método incorrecto");
            return false;
        }

        // 🔥 IMPORTANTE:
        // ❌ NO VALIDAMOS INPUT EN INVENTARIO
        // ✔ porque ya fue consumido en el DROP

        // 🔥 VALIDAR ESPACIO PARA OUTPUTS
        var add = compound.outputs
            .Where(o => o.item != null && o.amount > 0)
            .Select(o => (o.item, o.amount))
            .ToArray();

        bool canAdd = read.CanAddItemsBatch(add);

        if (!canAdd)
        {
            Debug.LogWarning("CanSeparate: no hay espacio para outputs");
        }

        return canAdd;
    }

    // =========================================================
    // EJECUCIÓN
    // =========================================================
    public bool Execute(
        CompoundDefinition_SO compound,
        SeparationMethod_SO method,
        IInventoryReadModel read,
        IInventoryWriteModel write)
    {

        if (compound == null)
            return false;
        
        // VALIDAR ANTES DE EJECUTAR
        if (!CanSeparate(compound, method, read))
            return false;
        

        var remove = new (ItemData_SO item, int amount)[0];
        var add = compound.outputs
            .Where(output => output.item != null && output.amount > 0)
            .Select(output => (output.item, output.amount))
            .ToArray();

        if (!write.TryProcessBatch(remove, add))
            return false;

        Debug.Log("✔ Separación completada");

        return true;
    }
}
