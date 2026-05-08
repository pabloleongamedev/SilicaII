/*
 * Arquitectura: Inventory/Core
 * Script: InventoryFeedback
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
public struct InventoryFeedback
{
    public string message;
    public InventoryFeedbackType type;
}

public enum InventoryFeedbackType
{
    Info,
    Success,
    Warning,
    Error
}
