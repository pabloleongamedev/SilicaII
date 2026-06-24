using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Events/Notification State Event Channel")]
public class NotificationStateEventChannel_SO : ScriptableObject
{
    public event Action<bool> Raised;

    public void Raise(bool isVisible)
    {
        Raised?.Invoke(isVisible);
    }
}
