/*
 * Arquitectura: Interaction/Core
 * Script: IUIStateInteractable
 * Rol: Contrato opcional para declarar en que UIState un interactuable acepta interaccion.
 * Relaciones: PlayerStateController consulta este contrato sin conocer tipos concretos como CraftingTable o ChemistryTable.
 * Uso como referencia: mantiene IInteractable enfocado en la accion y separa las reglas de disponibilidad por estado UI.
 */
public interface IUIStateInteractable
{
    bool CanInteractInState(UIState currentState);
}
