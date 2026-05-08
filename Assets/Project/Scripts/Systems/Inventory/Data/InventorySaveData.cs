/*
 * Arquitectura: Inventory/Data
 * Script: InventorySaveData
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
/// <summary>
/// Datos serializables de un item en el inventario.
/// </summary>
[System.Serializable]
public class InventorySaveData
{
    public string itemID;
    public int gridX;
    public int gridY;
    public int quantity = 1;
}
