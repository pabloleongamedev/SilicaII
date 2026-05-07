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