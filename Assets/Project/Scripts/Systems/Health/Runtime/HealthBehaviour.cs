using UnityEngine;

public class HealthBehaviour : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;

    private HealthComponent health;

    public HealthComponent Health => health;

    private void Awake()
    {
        health = new HealthComponent(maxHealth);
    }

    public void ReceiveDamage(in DamageContext context)
    {
        health.ReceiveDamage(context);
    }
}