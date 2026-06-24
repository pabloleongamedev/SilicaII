/*
 * Arquitectura: Health/Runtime
 * Script: HealthBehaviour
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona dano, salud y componentes que pueden recibir dano.
 * Relaciones: Recibe dano desde fuentes como DamageOverTimeSource o proyectiles mediante IDamageable/DamageDispatcher; expone eventos para UI y adapters como PlayerDeathHandler.
 * Fase 3: Health ya no decide GameOver ni notificaciones; solo publica OnDied para mantener el sistema reutilizable en jugador, enemigos u objetos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using System;

public class HealthBehaviour : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;

    private HealthComponent health;

    public HealthComponent Health
    {
        get
        {
            EnsureInitialized();
            return health;
        }
    }

    public int CurrentHealth
    {
        get
        {
            EnsureInitialized();
            return health.Current;
        }
    }

    public int MaxHealth
    {
        get
        {
            EnsureInitialized();
            return health.Max;
        }
    }

    public float HealthRatio
    {
        get
        {
            EnsureInitialized();
            return health.GetHealthRatio();
        }
    }

    public event Action<int, int> OnHealthChanged;
    public event Action<DamageContext> OnDamaged;
    public event Action OnDied;

    private void Awake()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (health != null)
            return;

        health = new HealthComponent(maxHealth);
        health.OnHealthChanged += HandleHealthChanged;
        health.OnDamaged += HandleDamaged;
        health.OnDied += HandleDied;
    }

    private void OnDestroy()
    {
        if (health == null)
            return;

        health.OnHealthChanged -= HandleHealthChanged;
        health.OnDamaged -= HandleDamaged;
        health.OnDied -= HandleDied;
    }

    public void ReceiveDamage(in DamageContext context)
    {
        EnsureInitialized();
        health.ReceiveDamage(context);
    }

    public void Heal(int amount)
    {
        EnsureInitialized();
        health.Heal(amount);
    }

    public void RestoreHealth(int current, int max)
    {
        // SaveLoad restaura estado sin emitir muerte; morir es gameplay, no hidratacion de datos.
        EnsureInitialized();
        health.RestoreState(current, max);
    }

    private void HandleHealthChanged(int current, int max)
    {
        // Adapter Runtime -> UI: HealthHUDPresenter escucha esta senal sin conocer HealthComponent interno.
        OnHealthChanged?.Invoke(current, max);
    }

    private void HandleDamaged(DamageContext context)
    {
        OnDamaged?.Invoke(context);
    }

    private void HandleDied()
    {
        // Health publica la muerte; otros adapters deciden consecuencias como GameOver, audio o UI.
        OnDied?.Invoke();
    }
}
