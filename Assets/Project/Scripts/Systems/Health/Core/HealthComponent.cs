/*
 * Arquitectura: Health/Core
 * Script: HealthComponent
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona dano, salud y componentes que pueden recibir dano.
 * Relaciones: Colabora con cualquier sistema que aplique dano mediante IDamageable y DamageDispatcher.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;
using UnityEngine;
public class HealthComponent : IDamageable
{
    public int Current { get; private set; }
    public int Max { get; private set; }

    public bool IsDead => Current <= 0;

    public event Action<int, int> OnHealthChanged;
    public event Action<DamageContext> OnDamaged;
    public event Action OnDied;

    public HealthComponent(int maxHealth)
    {
        Max = maxHealth;
        Current = maxHealth;
    }

    public void ReceiveDamage(in DamageContext context)
    {
        if (IsDead || context.Amount <= 0)
            return;

        // Core mantiene el estado canonico de salud. La UI escucha OnHealthChanged,
        // pero no decide reglas de dano ni game over.
        Current = Mathf.Max(Current - context.Amount, 0);

        OnDamaged?.Invoke(context);
        OnHealthChanged?.Invoke(Current, Max);

        if (IsDead)
        {
            OnDied?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead)
            return;

        Current = Mathf.Min(Current + amount, Max);
        OnHealthChanged?.Invoke(Current, Max);
    }

    public void RestoreState(int current, int max)
    {
        Max = Mathf.Max(1, max);
        Current = Mathf.Clamp(current, 0, Max);
        OnHealthChanged?.Invoke(Current, Max);
    }

    public float GetHealthRatio()
    {
        return Max <= 0 ? 0f : Mathf.Clamp01((float)Current / Max);
    }
}
