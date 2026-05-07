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