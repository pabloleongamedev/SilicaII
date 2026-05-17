/*
 * Arquitectura: SaveLoad/Runtime
 * Script: JetpackSaveParticipant
 * Rol: Participante dedicado a persistir combustible actual del jetpack.
 * Relaciones: Depende de MovementController como facade publica; no conoce JetpackSystem interno.
 * Fase desacople: no usa busquedas globales; la referencia al lector de fuel debe ser visible en escena.
 */
using UnityEngine;

public class JetpackSaveParticipant : MonoBehaviour, ISaveParticipant
{
    [SerializeField] private MovementController movement;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<MovementController>();
    }

    public void Capture(GameData gameData)
    {
        if (gameData == null || movement == null)
        {
            WarnMissingMovement();
            return;
        }

        gameData.playerData.jetpackFuel = movement.GetJetpackFuel();
    }

    public void Restore(GameData gameData, IItemResolver itemResolver)
    {
        if (gameData == null || movement == null)
        {
            WarnMissingMovement();
            return;
        }

        movement.RestoreJetpackFuel(gameData.playerData.jetpackFuel);
    }

    private void WarnMissingMovement()
    {
        if (movement == null)
            Debug.LogWarning("[JetpackSaveParticipant] Asigna MovementController por Inspector o ubica el participante en el Player.", this);
    }
}
