/*
 * Arquitectura: Notification/Data
 * Script: NotificationData
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona mensajes visuales y sonoros de feedback para el jugador.
 * Relaciones: Viaja por NotificationEventChannel_SO entre sistemas productores y vistas/audio de notificacion.
 * Fase 2: DTO de notificacion queda asociado al canal de evento de Notification.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
}

[System.Serializable]
public struct NotificationData
{
    public string message;
    public NotificationType type;
}
