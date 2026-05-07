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

        GameplayEvents.OnGameStateChanged?.Invoke(currentState);
    }

    public bool IsBlocked()
    {
        return currentState != GameState.Gameplay;
    }
}