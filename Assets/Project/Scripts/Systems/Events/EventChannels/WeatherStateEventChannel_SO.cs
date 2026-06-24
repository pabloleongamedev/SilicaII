using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Events/Weather State Event Channel")]
public class WeatherStateEventChannel_SO : ScriptableObject
{
    public event Action<bool> Raised;

    public void Raise(bool isRaining)
    {
        Raised?.Invoke(isRaining);
    }
}
