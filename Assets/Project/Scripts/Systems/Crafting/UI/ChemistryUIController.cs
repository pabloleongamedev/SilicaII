/*
 * Arquitectura: Crafting/UI
 * Script: ChemistryUIController
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class ChemistryUIController : MonoBehaviour
{
    [SerializeField] private GameObject chemistryPanel;
    [SerializeField] private UIStateEventChannel_SO uiStateChannel;

    private void OnEnable()
    {
        if (uiStateChannel != null)
            uiStateChannel.Raised += HandleState;
    }

    private void OnDisable()
    {
        if (uiStateChannel != null)
            uiStateChannel.Raised -= HandleState;
    }

    private void HandleState(UIState state)
    {
        if (chemistryPanel == null) return;

        bool isActive = state == UIState.Chemistry;

        chemistryPanel.SetActive(isActive);
    }
}
