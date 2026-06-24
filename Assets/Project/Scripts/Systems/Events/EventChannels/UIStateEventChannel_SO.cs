/*
 * Arquitectura: Events/EventChannels
 * Script: UIStateEventChannel_SO
 * Rol: Canal de evento serializable para bloqueos o cambios de estado UI/player.
 * Relaciones: Salida oficial de PlayerStateController para pausa, tutorial y paneles UI asignados por Inspector.
 * Riesgo arquitectonico mitigado: permite multiples contextos de UI sin compartir un bus estatico global.
 */
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Events/UI State Event Channel")]
public class UIStateEventChannel_SO : ScriptableObject
{
    public event Action<UIState> Raised;

    public void Raise(UIState state)
    {
        Raised?.Invoke(state);
    }
}
