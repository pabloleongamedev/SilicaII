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