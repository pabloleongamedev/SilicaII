/*
 * Arquitectura: SaveLoad/Runtime
 * Script: PlayerVitalsSaveSection
 * Rol: Seccion no-MonoBehaviour para persistir salud actual/maxima del Player.
 * Relaciones: Usada por PlayerSaveParticipant; depende solo de HealthBehaviour como facade de Health.
 */
using UnityEngine;

public class PlayerVitalsSaveSection : ISaveSection
{
    private readonly HealthBehaviour health;
    private readonly Object logContext;

    public PlayerVitalsSaveSection(HealthBehaviour health, Object logContext)
    {
        this.health = health;
        this.logContext = logContext;
    }

    public void Capture(GameData gameData)
    {
        if (gameData == null || health == null)
        {
            WarnMissingHealth();
            return;
        }

        gameData.playerData.health = health.CurrentHealth;
        gameData.playerData.maxHealth = health.MaxHealth;
    }

    public void Restore(GameData gameData, IItemResolver itemResolver)
    {
        if (gameData == null || health == null)
        {
            WarnMissingHealth();
            return;
        }

        health.RestoreHealth(gameData.playerData.health, gameData.playerData.maxHealth);
    }

    private void WarnMissingHealth()
    {
        if (health == null)
            Debug.LogWarning("[PlayerVitalsSaveSection] Falta HealthBehaviour en PlayerSaveParticipant.", logContext);
    }
}
