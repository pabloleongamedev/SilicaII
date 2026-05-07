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