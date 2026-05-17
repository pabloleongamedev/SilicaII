/*
 * Arquitectura: SaveLoad/Runtime
 * Script: PlayerMissionTimerSaveSection
 * Rol: Seccion no-MonoBehaviour para persistir tiempo restante de mision asociado a la sesion del Player.
 * Relaciones: Usada por PlayerSaveParticipant; depende solo de MissionTimer como facade de tiempo.
 */
using UnityEngine;

public class PlayerMissionTimerSaveSection : ISaveSection
{
    private readonly MissionTimer missionTimer;
    private readonly Object logContext;

    public PlayerMissionTimerSaveSection(MissionTimer missionTimer, Object logContext)
    {
        this.missionTimer = missionTimer;
        this.logContext = logContext;
    }

    public void Capture(GameData gameData)
    {
        if (gameData == null || missionTimer == null)
        {
            WarnMissingTimer();
            return;
        }

        gameData.missionTimeRemaining = missionTimer.CurrentTime;
        gameData.missionDuration = missionTimer.MissionDuration;
    }

    public void Restore(GameData gameData, IItemResolver itemResolver)
    {
        if (gameData == null || missionTimer == null || gameData.missionTimeRemaining < 0f)
        {
            if (missionTimer == null)
                WarnMissingTimer();

            return;
        }

        missionTimer.RestoreTime(gameData.missionTimeRemaining);
    }

    private void WarnMissingTimer()
    {
        if (missionTimer == null)
            Debug.LogWarning("[PlayerMissionTimerSaveSection] Falta MissionTimer en PlayerSaveParticipant.", logContext);
    }
}
