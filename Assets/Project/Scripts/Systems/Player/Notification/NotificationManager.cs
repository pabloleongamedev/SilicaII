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