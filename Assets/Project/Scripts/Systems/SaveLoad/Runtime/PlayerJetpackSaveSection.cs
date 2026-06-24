/*
 * Arquitectura: SaveLoad/Runtime
 * Script: PlayerJetpackSaveSection
 * Rol: Seccion no-MonoBehaviour para persistir combustible actual del jetpack.
 * Relaciones: Usada por PlayerSaveParticipant; depende de MovementController como facade de IJetpackFuelReader/restauracion.
 */
using UnityEngine;

public class PlayerJetpackSaveSection : ISaveSection
{
    private readonly MovementController movement;
    private readonly Object logContext;

    public PlayerJetpackSaveSection(MovementController movement, Object logContext)
    {
        this.movement = movement;
        this.logContext = logContext;
    }

    public void Capture(GameData gameData)
    {
        if (gameData == null || movement == null)
        {
            WarnMissingMovement();
            return;
        }

        gameData.playerData.jetpackFuel = movement.GetCurrentFuel();
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
            Debug.LogWarning("[PlayerJetpackSaveSection] Falta MovementController en PlayerSaveParticipant.", logContext);
    }
}
