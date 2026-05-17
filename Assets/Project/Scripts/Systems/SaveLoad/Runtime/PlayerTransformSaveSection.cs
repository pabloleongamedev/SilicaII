/*
 * Arquitectura: SaveLoad/Runtime
 * Script: PlayerTransformSaveSection
 * Rol: Seccion no-MonoBehaviour para persistir posicion y rotacion del Player.
 * Relaciones: Usada por PlayerSaveParticipant; no conoce Inventory, Health, Jetpack ni Timer.
 */
using UnityEngine;

public class PlayerTransformSaveSection : ISaveSection
{
    private readonly PlayerInputHandler player;
    private readonly Object logContext;

    public PlayerTransformSaveSection(PlayerInputHandler player, Object logContext)
    {
        this.player = player;
        this.logContext = logContext;
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
            return;
        }

        player.transform.SetPositionAndRotation(position, rotation);
    }

    private void WarnMissingPlayer()
    {
        if (player == null)
            Debug.LogWarning("[PlayerTransformSaveSection] Falta PlayerInputHandler en PlayerSaveParticipant.", logContext);
    }
}
