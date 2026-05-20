/*
 * Arquitectura: Notification/Runtime
 * Script: NotificationAudioController
 * Rol: Adaptador de audio de notificaciones.
 * Modulo: Gestiona mensajes visuales y sonoros de feedback para el jugador.
 * Relaciones: Escucha NotificationEvents.OnNotification y solicita AudioCueKey al IAudioService central.
 * Uso como referencia: no guarda clips propios; AudioCueLibrary_SO centraliza las referencias.
 */
using UnityEngine;

public class NotificationAudioController : MonoBehaviour
{
    [Header("Audio Service")]
    [SerializeField] private MonoBehaviour audioServiceBehaviour;

    private IAudioService audioService;

    private void OnEnable()
    {
        ResolveAudioService();
        NotificationEvents.OnNotification += HandleNotification;
    }

    private void OnDisable()
    {
        NotificationEvents.OnNotification -= HandleNotification;
    }

    private void HandleNotification(NotificationData data)
    {
        audioService?.Play(GetCueKey(data.type));
    }

    private AudioCueKey GetCueKey(NotificationType type)
    {
        switch (type)
        {
            case NotificationType.Success: return AudioCueKey.NotificationSuccess;
            case NotificationType.Error: return AudioCueKey.NotificationError;
            case NotificationType.Warning: return AudioCueKey.NotificationWarning;
            case NotificationType.Info: return AudioCueKey.NotificationInfo;
            default: return AudioCueKey.NotificationInfo;
        }
    }

    private void ResolveAudioService()
    {
        if (audioServiceBehaviour == null)
            audioServiceBehaviour = GetComponentInParent<AudioService>();

        audioService = audioServiceBehaviour as IAudioService;

        if (audioService == null && audioServiceBehaviour != null)
            Debug.LogWarning("[NotificationAudioController] El Audio Service asignado no implementa IAudioService.", this);
    }
}
