/*
 * Arquitectura: Health/Runtime
 * Script: DamageOverTimeSource
 * Rol: Fuente generica de dano periodico para cualquier IDamageable.
 * Modulo: Permite que un cubo, zona, hazard, proyectil lento o sistema temporal consuma vida sin conocer HealthComponent interno.
 * Relaciones: Aplica DamageContext mediante DamageDispatcher; HealthBehaviour recibe el dano y decide vida/muerte/notificacion.
 * Uso como referencia: si hay Target asignado, dana ese objetivo cada tick; si no hay Target, funciona como zona trigger.
 */
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTimeSource : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float tickInterval = 1f;

    [Header("Optional Direct Target")]
    [SerializeField] private GameObject target;

    private readonly Dictionary<GameObject, float> nextTickByTarget = new Dictionary<GameObject, float>();
    private float nextDirectTargetTick;

    private void Update()
    {
        if (target == null)
            return;

        if (Time.time < nextDirectTargetTick)
            return;

        nextDirectTargetTick = Time.time + Mathf.Max(0.01f, tickInterval);
        ApplyDamage(target);
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject damageTarget = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (damageTarget == null)
            return;

        float nextTick;
        nextTickByTarget.TryGetValue(damageTarget, out nextTick);

        if (Time.time < nextTick)
            return;

        nextTickByTarget[damageTarget] = Time.time + Mathf.Max(0.01f, tickInterval);
        ApplyDamage(damageTarget);
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject damageTarget = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (damageTarget != null)
            nextTickByTarget.Remove(damageTarget);
    }

    private void ApplyDamage(GameObject damageTarget)
    {
        if (damagePerTick <= 0)
            return;

        var context = new DamageContext(
            damagePerTick,
            transform.position,
            transform.forward,
            gameObject,
            "DamageOverTime");

        DamageDispatcher.ApplyDamage(damageTarget, context);
    }
}
