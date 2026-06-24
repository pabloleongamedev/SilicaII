/*
 * Arquitectura: Inventory/Core
 * Script: IInventoryReadModel
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;

public interface IInventoryReadModel
{
    int Capacity { get; }

    InventoryItemInstance GetItem(int index);

    bool CanAddItem(ItemData_SO item, int amount);

    //  EXISTENTE
    bool CanAddItemsBatch(params (ItemData_SO item, int amount)[] items);

    //  NUEVO (CLAVE PARA CRAFTING)
    bool CanProcessBatch(
        (ItemData_SO item, int amount)[] remove,
        (ItemData_SO item, int amount)[] add
    );
    int GetAmount(ItemData_SO item); 

    event Action<int, InventoryItemInstance> OnItemChanged;
}