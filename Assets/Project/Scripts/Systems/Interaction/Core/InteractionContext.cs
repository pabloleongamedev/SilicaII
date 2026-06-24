/*
 * Arquitectura: Interaction/Core
 * Script: InteractionContext
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona deteccion, contexto y ejecucion de interacciones del jugador con objetos del mundo.
 * Relaciones: Usa IInteractable e InteractionContext para conectar jugador, mundo e Inventory sin dependencias profundas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
/// <summary>
/// Contiene dependencias necesarias para interactuar.
/// Evita usar GetComponent dentro de cada interactuable.
/// </summary>
public class InteractionContext
{
    // Paquete explicito de dependencias para IInteractable.
    // Evita busquedas globales y mantiene las interacciones testeables.
    public IInventoryReadModel InventoryRead { get; private set; }
    public IInventoryWriteModel InventoryWrite { get; private set; }

    public InteractionContext(IInventoryReadModel inventoryRead, IInventoryWriteModel inventoryWrite)
    {
        InventoryRead = inventoryRead;
        InventoryWrite = inventoryWrite;
    }

    public InteractionContext(InventorySystem inventory)
        : this(inventory.ReadModel, inventory)
    {
    }
}
