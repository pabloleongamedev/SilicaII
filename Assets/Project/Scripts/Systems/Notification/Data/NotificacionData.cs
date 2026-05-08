/*
 * Arquitectura: Notification/Data
 * Script: NotificacionData
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona mensajes visuales y sonoros de feedback para el jugador.
 * Relaciones: Escucha GameplayEvents y bloquea temporalmente Interaction UI mientras muestra feedback.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

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
    public AudioClip sound;
}