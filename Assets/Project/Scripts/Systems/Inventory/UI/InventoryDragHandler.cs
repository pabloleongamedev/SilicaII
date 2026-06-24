/*
 * Arquitectura: Inventory/UI
 * Script: InventoryDragHandler
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.UI;

public class InventoryDragHandler : MonoBehaviour
{
    [SerializeField] private Image ghostIcon;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        ghostIcon.gameObject.SetActive(false);
    }

    public void StartDrag(Sprite icon)
    {
        ghostIcon.transform.SetAsLastSibling();
        ghostIcon.sprite = icon;
        ghostIcon.gameObject.SetActive(true);
    }

        public void UpdateDrag(Vector2 position)
        {
            ghostIcon.rectTransform.position = position;
        }

    public void EndDrag()
    {
        ghostIcon.gameObject.SetActive(false);
    }
    
}