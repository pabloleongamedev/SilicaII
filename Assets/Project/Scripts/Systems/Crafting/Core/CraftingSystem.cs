/*
 * Arquitectura: Crafting/Core
 * Script: CraftingSystem
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem
{
    private RecipeData_SO currentRecipe;

    private Dictionary<int, (ItemData_SO item, int amount)> slots = new();

    public void SetRecipe(RecipeData_SO recipe)
    {
        currentRecipe = recipe;
        slots.Clear();
    }

    public CraftingResult TryPlaceItem(int slotIndex, ItemData_SO item, IInventoryReadModel read, IInventoryWriteModel write)
    {
        if (slots.ContainsKey(slotIndex))
            return CraftingResult.Fail("No puedes agregar mas elementos.");

        if (currentRecipe == null)
            return CraftingResult.Fail("No hay una formula seleccionada");

        var ingredient = currentRecipe.ingredients
            .Find(x => x.item.itemID == item.itemID);

        if (ingredient == null)
            return CraftingResult.Fail("Este elemento no pertenece a la formula");

        int current = GetCurrentAmount(item);
        int required = ingredient.amount;

        if (current >= required)
            return CraftingResult.Fail("Ya agregaste los elementos necesarios de este tipo");

        int remaining = required - current;
        int available = read.GetAmount(item);

        if (available < remaining)
            return CraftingResult.Fail("No hay suficientes elementos en el inventario");

        write.RemoveItem(item, remaining);

        slots[slotIndex] = (item, remaining);

        return CraftingResult.Success($"Agregaste  {item.itemID}  x{remaining}");
    }

    public void ClearAllNoReturn()
    {
        slots.Clear();
    }

    public int GetCurrentAmount(ItemData_SO item)
    {
        int total = 0;

        foreach (var s in slots.Values)
        {
            if (s.item == item)
                total += s.amount;
        }

        return total;
    }

    public void ClearSlot(int index, IInventoryWriteModel write)
    {
        if (!slots.ContainsKey(index))
            return;

        var data = slots[index];

        write.AddItem(data.item, data.amount);

        slots.Remove(index);
    }

    public void ClearAll(IInventoryWriteModel write)
    {
        foreach (var slot in slots.Values)
        {
            write.AddItem(slot.item, slot.amount);
        }

        slots.Clear();
    }

    public bool IsRecipeComplete()
    {
        if (currentRecipe == null)
            return false;

        foreach (var ing in currentRecipe.ingredients)
        {
            if (GetCurrentAmount(ing.item) < ing.amount)
                return false;
        }

        return true;
    }

    public (ItemData_SO item, int amount)[] BuildRemoveBatch()
    {
        if (currentRecipe == null)
            return new (ItemData_SO, int)[0];

        var list = new List<(ItemData_SO, int)>();

        foreach (var ing in currentRecipe.ingredients)
        {
            list.Add((ing.item, ing.amount));
        }

        return list.ToArray();
    }

    public RecipeData_SO GetCurrentRecipe()
    {
        return currentRecipe;
    }
}
