/*
 * Arquitectura: Crafting/UI
 * Script: ChemistrySlotView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.UI;

public class ChemistrySlotView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject emptyState;

    public void SetItem(ItemData_SO item)
    {
        icon.sprite = item.icon;
        icon.enabled = true;
        emptyState.SetActive(false);
    }

    public void Clear()
    {
        icon.sprite = null;
        icon.enabled = false;
        emptyState.SetActive(true);
    }
}