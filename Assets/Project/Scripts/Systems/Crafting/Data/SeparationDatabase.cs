/*
 * Arquitectura: Crafting/Data
 * Script: SeparationDatabase
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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
