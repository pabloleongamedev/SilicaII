using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject crosshair;

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
        if (crosshair == null) return;

        // 🔥 visible SOLO cuando no hay UI
        crosshair.SetActive(state == UIState.None);
    }
}