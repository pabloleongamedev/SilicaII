/*
 * Arquitectura: Crafting/UI
 * Script: CraftingUIController
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class CraftingUIController : MonoBehaviour
{
    [SerializeField] private GameObject craftingPanel;

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
        if (craftingPanel == null) return;

        bool isActive = state == UIState.Crafting;

        craftingPanel.SetActive(isActive);
    }
}