/*
 * Arquitectura: Menu/Core
 * Script: IGamePauseService
 * Rol: Contrato para pausar/reanudar simulacion sin que la UI manipule Time.timeScale directamente.
 * Relaciones: PauseMenuManager y PlayerStateController consumen este contrato por Inspector.
 */
public interface IGamePauseService
{
    void SetPaused(bool paused);
}
