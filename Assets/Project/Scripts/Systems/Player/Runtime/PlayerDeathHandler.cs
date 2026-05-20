/*
 * Arquitectura: Player/Runtime
 * Script: PlayerDeathHandler
 * Rol: Adapter entre HealthBehaviour.OnDied y las consecuencias especificas del jugador.
 * Relaciones: Escucha HealthBehaviour, cambia GameStateController a GameOver y publica NotificationEvents.
 * Requisito de escena: debe vivir en el prefab Player junto a HealthBehaviour para que la muerte dispare GameOver.
 * Fase 3: separa la regla "si el jugador muere, hay GameOver" del sistema Health, que debe poder reutilizarse en enemigos/objetos.
 */
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(HealthBehaviour))]
public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private HealthBehaviour health;
    [SerializeField] private GameStateController gameStateController;
    [SerializeField] private bool notifyOnDeath = true;
    [SerializeField] private string deathMessage = "Integridad agotada. Game Over.";

    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthBehaviour>();

        if (gameStateController == null)
            gameStateController = GetComponent<GameStateController>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDied -= HandlePlayerDied;
    }

    private void HandlePlayerDied()
    {
        if (gameStateController != null)
            gameStateController.SetState(GameState.GameOver);
        else
            GameStateEvents.Publish(GameState.GameOver);

        if (!notifyOnDeath)
            return;

        NotificationEvents.PublishNotification(new NotificationData
        {
            message = deathMessage,
            type = NotificationType.Error
        });
    }
}
