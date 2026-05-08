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