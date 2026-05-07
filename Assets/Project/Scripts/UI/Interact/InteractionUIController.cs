using TMPro;
using UnityEngine;

public class InteractionUIController : MonoBehaviour
{
    [SerializeField] private InteractionDetector detector;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text text;

    private bool notificationActive;

    private void OnEnable()
    {
        if (detector != null)
            detector.OnInteractableChanged += HandleChanged;

        GameplayEvents.OnNotificationStateChanged += HandleNotification;
    }

    private void OnDisable()
    {
        if (detector != null)
            detector.OnInteractableChanged -= HandleChanged;

        GameplayEvents.OnNotificationStateChanged -= HandleNotification;
    }

    private void HandleNotification(bool isActive)
    {
        notificationActive = isActive;
        Refresh();
    }

    private void HandleChanged(IInteractable interactable)
    {
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        // 🔥 PRIORIDAD: NOTIFICACIÓN
        if (notificationActive)
        {
            Hide();
            return;
        }

        var interactable = detector.CurrentInteractable;

        if (interactable == null)
        {
            Hide();
            return;
        }

        string msg = interactable.GetInteractionText();

        if (string.IsNullOrEmpty(msg))
        {
            Hide();
            return;
        }

        Show(msg);
    }

    private void Show(string msg)
    {
        if (!panel.activeSelf)
            panel.SetActive(true);

        text.text = msg;
    }

    private void Hide()
    {
        if (panel.activeSelf)
            panel.SetActive(false);
    }
}