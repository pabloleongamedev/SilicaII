using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Events/Scanner Feedback Event Channel")]
public class ScannerFeedbackEventChannel_SO : ScriptableObject
{
    public event Action<IScannable> Raised;

    public void Raise(IScannable scannable)
    {
        Raised?.Invoke(scannable);
    }
}
