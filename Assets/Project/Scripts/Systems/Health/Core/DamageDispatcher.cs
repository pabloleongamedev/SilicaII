/*
 * Arquitectura: Health/Core
 * Script: DamageDispatcher
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona dano, salud y componentes que pueden recibir dano.
 * Relaciones: Colabora con cualquier sistema que aplique dano mediante IDamageable y DamageDispatcher.
 * Nota: busca IDamageable en el target, sus hijos y sus padres para soportar colliders en hijos con HealthBehaviour en el root.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using System.Collections.Generic;

public static class DamageDispatcher
{
    public static void ApplyDamage(GameObject target, in DamageContext context)
    {
        if (target == null)
            return;

        var damageables = new List<IDamageable>();
        AddUnique(damageables, target.GetComponents<IDamageable>());
        AddUnique(damageables, target.GetComponentsInChildren<IDamageable>());
        AddUnique(damageables, target.GetComponentsInParent<IDamageable>());

        foreach (var d in damageables)
        {
            d.ReceiveDamage(context);
        }
    }

    private static void AddUnique(List<IDamageable> damageables, IDamageable[] candidates)
    {
        foreach (var candidate in candidates)
        {
            if (candidate != null && !damageables.Contains(candidate))
                damageables.Add(candidate);
        }
    }
}
