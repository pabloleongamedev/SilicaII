/*
 * Arquitectura: Interaction/UI
 * Script: InteractionUIController
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona deteccion, contexto y ejecucion de interacciones del jugador con objetos del mundo.
 * Relaciones: Usa IInteractable e InteractionContext para conectar jugador, mundo e Inventory sin dependencias profundas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using TMPro;
using UnityEngine;

public class InteractionUIController : MonoBehaviour
{
    [SerializeField] private InteractionDetector detector;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text text;
    [SerializeField] private NotificationStateEventChannel_SO notificationStateChannel;

    private bool notificationActive;

    private void OnEnable()
    {
        if (detector != null)
            detector.OnInteractableChanged += HandleChanged;

        if (notificationStateChannel != null)
            notificationStateChannel.Raised += HandleNotification;
    }

    private void OnDisable()
    {
        if (detector != null)
            detector.OnInteractableChanged -= HandleChanged;

        if (notificationStateChannel != null)
            notificationStateChannel.Raised -= HandleNotification;
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
