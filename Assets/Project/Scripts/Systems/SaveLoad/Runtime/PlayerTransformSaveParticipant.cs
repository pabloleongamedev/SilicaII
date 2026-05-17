/*
 * Arquitectura: SaveLoad/Runtime
 * Script: PlayerTransformSaveParticipant
 * Rol: Participante dedicado a persistir posicion y rotacion del jugador.
 * Relaciones: SaveParticipantRegistry lo orquesta; no conoce Health, Inventory, Jetpack ni MissionTimer.
 * Fase desacople: no usa busquedas globales; la referencia al Player debe ser explicita o local.
 */
using UnityEngine;

public class PlayerTransformSaveParticipant : MonoBehaviour, ISaveParticipant
{
    [SerializeField] private PlayerInputHandler player;

    private void Awake()
    {
        if (player == null)
            player = GetComponent<PlayerInputHandler>();
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
            Debug.LogWarning("[PlayerTransformSaveParticipant] Asigna PlayerInputHandler por Inspector o ubica el participante en el Player.", this);
    }
}
