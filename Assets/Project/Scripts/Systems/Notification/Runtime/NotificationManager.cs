/*
 * Arquitectura: Notification/Runtime
 * Script: NotificationManager
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona mensajes visuales y sonoros de feedback para el jugador.
 * Relaciones: Escucha GameplayEvents y bloquea temporalmente Interaction UI mientras muestra feedback.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private NotificationView view;

    private void OnEnable()
    {
        GameplayEvents.OnNotification += Handle;
    }

    private void OnDisable()
    {
        GameplayEvents.OnNotification -= Handle;
    }

    private void Handle(NotificationData data)
    {
        if (view == null)
        {
            Debug.LogError("NotificationView no asignado");
            return;
        }

        view.Show(data);
    }
}