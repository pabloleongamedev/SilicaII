/*
 * Arquitectura: Crafting/UI
 * Script: ToolCraftingView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;
using UnityEngine;

public class ToolCraftingView : MonoBehaviour
{
    [SerializeField] private CraftingSlotView[] slots;

    public Action<int, ItemData_SO> OnItemDroppedInSlot;
    public Action<int, ItemData_SO> OnItemDragOut;

    private void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int index = i;

            slots[i].OnItemDropped += (slot, item) =>
            {
                OnItemDroppedInSlot?.Invoke(index, item);
            };

            slots[i].OnItemBeginDrag += (slot, item) =>
            {
                OnItemDragOut?.Invoke(index, item);
            };
        }
    }

    public void SetItemInSlot(int index, ItemData_SO item)
    {
        if (index < 0 || index >= slots.Length) return;
        slots[index].SetItem(item);
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        slots[index].Clear();
    }

    public void Clear()
    {
        foreach (var slot in slots)
            slot.Clear();
    }
    public bool IsComplete(RecipeData_SO recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            bool found = false;

            foreach (var slot in slots)
            {
                var item = slot.GetItem();

                if (item == null)
                    continue;

                if (item == ingredient.item)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;
        }

        return true;
    }
    public void ConsumeAllSlots()
    {
        foreach (var slot in slots)
        {
            slot.Clear();
        }
    }
}
