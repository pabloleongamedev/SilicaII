/*
 * Arquitectura: Inventory/UI
 * Script: DescriptionPanelView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionPanelView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;

        void Start()
        {
            Clear();
        }
        public void Show(InventoryItemInstance item)
    {

        Debug.Log("SHOW DESCRIPTION"); // 👈
        if (item == null)
        {
            Debug.Log("ITEM NULL, CLEARING"); // 👈
            Clear();
            return;
        }

        gameObject.SetActive(true);

        itemName.text = $"Simblo Quimico {item.Data.displayName}";
        itemDescription.text = item.Data.description;
        itemIcon.sprite = item.Data.icon;
        itemIcon.enabled = true;
    }
/*
    public void Show(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            Clear();
            return;
        }

        var data = slot.Item.Data;

        itemName.text = data.displayName;
        itemIcon.sprite = data.icon;
        itemIcon.enabled = true;
        itemDescription.text = data.description;
    }
*/
    public void Clear()
    {
        itemName.text = "";
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemDescription.text = "";

        gameObject.SetActive(false);
    }
    
}