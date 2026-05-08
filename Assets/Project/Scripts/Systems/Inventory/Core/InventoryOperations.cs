/*
 * Arquitectura: Inventory/Core
 * Script: InventoryOperations
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;
using UnityEngine;

public class InventoryOperations
{
    private InventoryGrid grid;

    public InventoryOperations(InventoryGrid grid)
    {
        this.grid = grid;
    }

    // =========================================================
    // ADD
    // =========================================================
    public int AddItem(ItemData_SO item, int amount, Action<int, int, InventoryItemInstance> onChanged)
    {
        int remaining = amount;

        // STACK
        foreach (var slot in grid.GetAllSlots())
        {
            if (remaining <= 0) break;

            if (!slot.IsEmpty && slot.ItemInstance.Data == item && !slot.ItemInstance.IsFull())
            {
                int added = slot.ItemInstance.Add(remaining);
                remaining -= added;

                if (added > 0)
                    onChanged?.Invoke(slot.X, slot.Y, slot.ItemInstance);
            }
        }

        // EMPTY
        foreach (var slot in grid.GetAllSlots())
        {
            if (remaining <= 0) break;

            if (slot.IsEmpty)
            {
                var instance = new InventoryItemInstance(item);

                int added = instance.Add(remaining);
                if (added <= 0) continue;

                slot.SetItem(instance);
                remaining -= added;

                onChanged?.Invoke(slot.X, slot.Y, slot.ItemInstance);
            }
        }

        return amount - remaining;
    }

    // =========================================================
    // MOVE
    // =========================================================
    public void Move(int fromX, int fromY, int toX, int toY, Action<int,int,InventoryItemInstance> onChanged)
    {
        if (fromX == toX && fromY == toY) return;

        var fromSlot = grid.GetSlot(fromX, fromY);
        var toSlot = grid.GetSlot(toX, toY);

        var fromItem = fromSlot.ItemInstance;
        var toItem = toSlot.ItemInstance;

        if (fromItem == toItem) return;

        fromSlot.SetItem(toItem);
        toSlot.SetItem(fromItem);

        onChanged?.Invoke(fromX, fromY, fromSlot.ItemInstance);
        onChanged?.Invoke(toX, toY, toSlot.ItemInstance);
    }

    // =========================================================
    // MERGE
    // =========================================================
    public void Merge(int fromX, int fromY, int toX, int toY, Action<int,int,InventoryItemInstance> onChanged)
    {
        var fromSlot = grid.GetSlot(fromX, fromY);
        var toSlot = grid.GetSlot(toX, toY);

        if (fromSlot.IsEmpty || toSlot.IsEmpty) return;

        var fromItem = fromSlot.ItemInstance;
        var toItem = toSlot.ItemInstance;

        if (fromItem.Data != toItem.Data) return;
        if (toItem.IsFull()) return;

        int moved = toItem.Add(fromItem.Quantity);
        if (moved <= 0) return;

        fromItem.Remove(moved);

        onChanged?.Invoke(toX, toY, toSlot.ItemInstance);

        if (fromItem.IsEmpty())
        {
            fromSlot.Clear();
            onChanged?.Invoke(fromX, fromY, null);
        }
        else
        {
            onChanged?.Invoke(fromX, fromY, fromSlot.ItemInstance);
        }
    }
}