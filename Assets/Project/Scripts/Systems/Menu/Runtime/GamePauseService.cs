/*
 * Arquitectura: Menu/Runtime
 * Script: GamePauseService
 * Rol: Adapter unico de escena para aplicar pausa global sobre Time.timeScale.
 * Relaciones: UI y PlayerState piden pausa por IGamePauseService; este service concentra el detalle de Unity.
 */
using UnityEngine;

public class GamePauseService : MonoBehaviour, IGamePauseService
{
    [SerializeField] private float pausedTimeScale = 0f;
    [SerializeField] private float runningTimeScale = 1f;

    public void SetPaused(bool paused)
    {
        Time.timeScale = paused ? pausedTimeScale : runningTimeScale;
    }
}
