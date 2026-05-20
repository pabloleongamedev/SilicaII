/*
 * Arquitectura: Notification/UI
 * Script: NotificationView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona mensajes visuales y sonoros de feedback para el jugador.
 * Relaciones: NotificationManager escucha NotificationEventChannel_SO; esta vista publica visibilidad por NotificationStateEventChannel_SO.
 * Fase 2: Notification depende de canales de dominio asignados por Inspector.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationView : MonoBehaviour
{
    [SerializeField] private GameObject panelNotification;   // 
    [SerializeField] private TMP_Text text;
    [SerializeField] private float duration = 2f;
    [SerializeField] private NotificationStateEventChannel_SO notificationStateChannel;

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
        notificationStateChannel?.Raise(true);

        text.text = data.message;

        yield return new WaitForSecondsRealtime(duration);

        // 🔥 limpiar
        text.text = string.Empty;

        // 🔥 OCULTAR PANEL
        panelNotification.SetActive(false);

        // 🔥 liberar interaction UI
        notificationStateChannel?.Raise(false);
    }
}
