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
        if (IsDead)
            return;

        Current -= context.Amount;

        OnDamaged?.Invoke(context);
        OnHealthChanged?.Invoke(Current, Max);

        if (Current <= 0)
        {
            Current = 0;
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
}