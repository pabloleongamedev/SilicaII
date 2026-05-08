/*
 * Arquitectura: Inventory/Core
 * Script: InventoryGrid
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System.Collections.Generic;

public class InventoryGrid
{
    private InventorySlot[,] grid;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public InventoryGrid(int width, int height)
    {
        Width = width;
        Height = height;

        grid = new InventorySlot[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new InventorySlot(x, y);
            }
        }
    }

    public bool TryFindFirstEmptySlot(out int outX, out int outY)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (grid[x, y].IsEmpty)
                {
                    outX = x;
                    outY = y;
                    return true;
                }
            }
        }

        outX = -1;
        outY = -1;
        return false;
    }

    public void SetItem(int x, int y, InventoryItemInstance item)
    {
        grid[x, y].SetItem(item);
    }

    public InventorySlot GetSlot(int x, int y)
    {
        return grid[x, y];
    }
    public InventoryGrid Clone()
    {
        var newGrid = new InventoryGrid(Width, Height);

        foreach (var slot in GetAllSlots())
        {
            if (!slot.IsEmpty)
            {
                var instance = new InventoryItemInstance(slot.ItemInstance.Data);
                instance.Add(slot.ItemInstance.Quantity);

                newGrid.GetSlot(slot.X, slot.Y).SetItem(instance);
            }
        }

        return newGrid;
    }

    public IEnumerable<InventorySlot> GetAllSlots()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                yield return grid[x, y];
            }
        }
    }

}