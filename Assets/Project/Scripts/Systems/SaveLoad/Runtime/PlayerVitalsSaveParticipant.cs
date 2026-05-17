/*
 * Arquitectura: SaveLoad/Runtime
 * Script: PlayerVitalsSaveParticipant
 * Rol: Participante dedicado a persistir la salud del jugador.
 * Relaciones: Depende solo de HealthBehaviour asignado por Inspector o ubicado en el mismo GameObject.
 * Fase desacople: no usa busquedas globales; las dependencias de guardado deben ser visibles en escena.
 */
using UnityEngine;

public class PlayerVitalsSaveParticipant : MonoBehaviour, ISaveParticipant
{
    [SerializeField] private HealthBehaviour health;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthBehaviour>();
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
            Debug.LogWarning("[PlayerVitalsSaveParticipant] Asigna HealthBehaviour por Inspector o ubica el participante en el Player.", this);
    }
}
