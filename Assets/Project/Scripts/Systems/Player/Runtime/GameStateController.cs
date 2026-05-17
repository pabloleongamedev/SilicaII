/*
 * Arquitectura: Player/Runtime
 * Script: GameStateController
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona estado global del jugador, input y bloqueos de gameplay/UI.
 * Relaciones: Coordina input, estados UI/gameplay y bloqueos globales usados por otros sistemas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    private GameState currentState = GameState.Gameplay;

    private void Awake()
    {
        Debug.Log("GameStateController inicializado");
    }

    public GameState GetState()
    {
        return currentState;
    }

    public void SetState(GameState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        Debug.Log("GAME STATE CAMBIADO A: " + currentState);

        GameStateEvents.OnGameStateChanged?.Invoke(currentState);
    }

    public bool IsBlocked()
    {
        return currentState != GameState.Gameplay;
    }
}
