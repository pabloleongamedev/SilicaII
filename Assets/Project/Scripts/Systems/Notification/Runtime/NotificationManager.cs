/*
 * Arquitectura: Notification/Runtime
 * Script: NotificationManager
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona mensajes visuales y sonoros de feedback para el jugador.
 * Relaciones: Escucha NotificationEvents.OnNotification y delega render a NotificationView.
 * Fase 2: Notification usa un canal propio en lugar de GameplayEvents.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private NotificationView view;

    private void OnEnable()
    {
        NotificationEvents.OnNotification += Handle;
    }

    private void OnDisable()
    {
        NotificationEvents.OnNotification -= Handle;
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
