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
        // 🔥 recorrer ingredientes
        foreach (var ingredient in recipe.ingredients)
        {
            bool found = false;

            // 🔥 buscar en TODOS los slots
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