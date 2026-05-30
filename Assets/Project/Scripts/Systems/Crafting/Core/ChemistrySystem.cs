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
/// Reglas de dominio para separacion quimica.
/// No depende de MonoBehaviour, UI ni input.
/// El input ya fue reservado por el flujo de drop.
/// </summary>
public class ChemistrySystem
{
    public bool CanSeparate(
        CompoundDefinition_SO compound,
        SeparationMethod_SO method,
        IInventoryReadModel read)
    {
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

        if (compound.requiredMethod != method)
        {
            Debug.LogWarning("CanSeparate: metodo incorrecto");
            return false;
        }

        // El input ya fue consumido al reservar el item en la mesa.
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

    public bool Execute(
        CompoundDefinition_SO compound,
        SeparationMethod_SO method,
        IInventoryReadModel read,
        IInventoryWriteModel write)
    {

        if (compound == null)
            return false;
        
        if (!CanSeparate(compound, method, read))
            return false;
        

        var remove = new (ItemData_SO item, int amount)[0];
        var add = compound.outputs
            .Where(output => output.item != null && output.amount > 0)
            .Select(output => (output.item, output.amount))
            .ToArray();

        if (!write.TryProcessBatch(remove, add))
            return false;

        Debug.Log("Separacion completada");

        return true;
    }
}
