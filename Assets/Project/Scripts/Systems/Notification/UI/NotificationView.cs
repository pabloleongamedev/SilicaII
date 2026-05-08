using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationView : MonoBehaviour
{
    [SerializeField] private GameObject panelNotification;   // 
    [SerializeField] private TMP_Text text;
    [SerializeField] private float duration = 2f;

    private Coroutine current;

    private void Awake()
    {
        // 🔥 asegurar estado inicial correcto
        if (panelNotification != null)
            panelNotification.SetActive(false);
    }

    public void Show(NotificationData data)
    {
        if (current != null)
            StopCoroutine(current);

        current = StartCoroutine(ShowRoutine(data));
    }

    private IEnumerator ShowRoutine(NotificationData data)
    {
        if (panelNotification == null || text == null)
            yield break;

        // 🔥 ACTIVAR PANEL
        panelNotification.SetActive(true);

        // 🔥 bloquear interaction UI
        GameplayEvents.OnNotificationStateChanged?.Invoke(true);

        text.text = data.message;

        yield return new WaitForSecondsRealtime(duration);

        // 🔥 limpiar
        text.text = string.Empty;

        // 🔥 OCULTAR PANEL
        panelNotification.SetActive(false);

        // 🔥 liberar interaction UI
        GameplayEvents.OnNotificationStateChanged?.Invoke(false);
    }
}