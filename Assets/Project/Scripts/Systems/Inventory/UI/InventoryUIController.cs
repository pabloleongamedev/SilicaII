/*
 * Arquitectura: Inventory/UI
 * Script: InventoryUIController
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject questPanel;

    private void OnEnable()
    {
        GameplayEvents.OnUIStateChanged += HandleState;
    }

    private void OnDisable()
    {
        GameplayEvents.OnUIStateChanged -= HandleState;
    }

    private void HandleState(UIState state)
    {
        if (inventoryPanel == null) return;
        // solo se activa si el estado es Inventory
        inventoryPanel.SetActive(state == UIState.Inventory);

        if (questPanel != null)
            questPanel.SetActive(state == UIState.Quest); 
    }
}