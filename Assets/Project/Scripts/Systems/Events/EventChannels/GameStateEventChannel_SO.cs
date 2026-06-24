/*
 * Arquitectura: Events/EventChannels
 * Script: GameStateEventChannel_SO
 * Rol: Canal de evento serializable para cambios de estado global de juego.
 * Relaciones: Salida oficial para GameStateController o presenters asignados por Inspector.
 * Riesgo arquitectonico mitigado: hace visible en escena que sistemas reaccionan a GameState sin depender de estado estatico compartido.
 */
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Events/Game State Event Channel")]
public class GameStateEventChannel_SO : ScriptableObject
{
    public event Action<GameState> Raised;

    public void Raise(GameState state)
    {
        Raised?.Invoke(state);
    }
}
