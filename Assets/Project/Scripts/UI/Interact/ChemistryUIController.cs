using UnityEngine;

public class ChemistryUIController : MonoBehaviour
{
    [SerializeField] private GameObject chemistryPanel;

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
        if (chemistryPanel == null) return;

        bool isActive = state == UIState.Chemistry;

        chemistryPanel.SetActive(isActive);
    }
}