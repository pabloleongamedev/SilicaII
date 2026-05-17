/*
 * Arquitectura: SaveLoad/Runtime
 * Script: MissionTimerSaveParticipant
 * Rol: Participante dedicado a persistir el tiempo restante de mision.
 * Relaciones: Depende de MissionTimer como facade de tiempo visible; no conoce HUDManager ni GameManager.
 * Fase desacople: no usa busquedas globales; la referencia al timer debe ser visible en escena.
 */
using UnityEngine;

public class MissionTimerSaveParticipant : MonoBehaviour, ISaveParticipant
{
    [SerializeField] private MissionTimer missionTimer;

    private void Awake()
    {
        if (missionTimer == null)
            missionTimer = GetComponent<MissionTimer>();
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
            Debug.LogWarning("[MissionTimerSaveParticipant] Asigna MissionTimer por Inspector o ubica el participante junto al timer.", this);
    }
}
