/*
 * Arquitectura: Notification/Runtime
 * Script: NotificationAudioController
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona mensajes visuales y sonoros de feedback para el jugador.
 * Relaciones: Escucha GameplayEvents y bloquea temporalmente Interaction UI mientras muestra feedback.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class NotificationAudioController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip errorClip;
    [SerializeField] private AudioClip warningClip;
    [SerializeField] private AudioClip infoClip;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameplayEvents.OnNotification += HandleNotification;
    }

    private void OnDisable()
    {
        GameplayEvents.OnNotification -= HandleNotification;
    }

    private void HandleNotification(NotificationData data)
    {
        if (audioSource == null) return;

        AudioClip clip = GetClip(data.type);

        if (clip == null) return;

        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetClip(NotificationType type)
    {
        switch (type)
        {
            case NotificationType.Success: return successClip;
            case NotificationType.Error: return errorClip;
            case NotificationType.Warning: return warningClip;
            case NotificationType.Info: return infoClip;
            default: return null;
        }
    }
}