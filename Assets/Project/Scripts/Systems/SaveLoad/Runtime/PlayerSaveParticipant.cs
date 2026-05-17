/*
 * Arquitectura: SaveLoad/Runtime
 * Script: PlayerSaveParticipant
 * Rol: Participante legacy de guardado para transform y estado runtime principal del jugador.
 * Relaciones: Mantiene compatibilidad con escenas que ya lo tienen asignado; los nuevos participantes dedicados son PlayerTransformSaveParticipant, PlayerVitalsSaveParticipant, JetpackSaveParticipant y MissionTimerSaveParticipant.
 * Fase desacople: no usa busquedas globales; queda solo como compatibilidad si sus referencias estan asignadas/locales.
 */
using UnityEngine;

public class PlayerSaveParticipant : MonoBehaviour, ISaveParticipant
{
    [SerializeField] private PlayerInputHandler player;
    [SerializeField] private HealthBehaviour health;
    [SerializeField] private MovementController movement;
    [SerializeField] private MissionTimer missionTimer;

    private void Awake()
    {
        ResolveLocalReferences();
    }

    public void Capture(GameData gameData)
    {
        if (gameData == null || player == null)
        {
            WarnMissingPlayer();
            return;
        }

        gameData.playerData.SetPosition(player.transform.position);
        gameData.playerData.SetRotation(player.transform.rotation);

        if (health != null)
        {
            gameData.playerData.health = health.CurrentHealth;
            gameData.playerData.maxHealth = health.MaxHealth;
        }

        if (movement != null)
            gameData.playerData.jetpackFuel = movement.GetJetpackFuel();

        if (missionTimer != null)
        {
            gameData.missionTimeRemaining = missionTimer.CurrentTime;
            gameData.missionDuration = missionTimer.MissionDuration;
        }
    }

    public void Restore(GameData gameData, IItemResolver itemResolver)
    {
        if (gameData == null || player == null)
        {
            WarnMissingPlayer();
            return;
        }

        var position = gameData.playerData.GetPosition();
        var rotation = gameData.playerData.GetRotation();

        if (player.TryGetComponent<Rigidbody>(out var rigidBody))
        {
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.position = position;
            rigidBody.rotation = rotation;
        }
        else
        {
            player.transform.SetPositionAndRotation(position, rotation);
        }

        if (health != null)
            health.RestoreHealth(gameData.playerData.health, gameData.playerData.maxHealth);

        if (movement != null)
            movement.RestoreJetpackFuel(gameData.playerData.jetpackFuel);

        if (missionTimer != null && gameData.missionTimeRemaining >= 0f)
            missionTimer.RestoreTime(gameData.missionTimeRemaining);
    }

    private void ResolveLocalReferences()
    {
        if (player == null)
            player = GetComponent<PlayerInputHandler>();

        if (player != null)
        {
            if (health == null)
                health = player.GetComponent<HealthBehaviour>();

            if (movement == null)
                movement = player.GetComponent<MovementController>();
        }

        if (missionTimer == null)
            missionTimer = GetComponent<MissionTimer>();
    }

    private void WarnMissingPlayer()
    {
        if (player == null)
            Debug.LogWarning("[PlayerSaveParticipant] Asigna PlayerInputHandler por Inspector o ubica el participante en el Player.", this);
    }
}
