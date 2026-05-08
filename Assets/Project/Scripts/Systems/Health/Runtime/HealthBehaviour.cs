/*
 * Arquitectura: Health/Runtime
 * Script: HealthBehaviour
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona dano, salud y componentes que pueden recibir dano.
 * Relaciones: Colabora con cualquier sistema que aplique dano mediante IDamageable y DamageDispatcher.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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