/*
 * Arquitectura: Health/Core
 * Script: DamageDispatcher
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona dano, salud y componentes que pueden recibir dano.
 * Relaciones: Colabora con cualquier sistema que aplique dano mediante IDamageable y DamageDispatcher.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
public static class DamageDispatcher
{
    public static void ApplyDamage(GameObject target, in DamageContext context)
    {
        if (target == null)
            return;

        var damageables = target.GetComponentsInChildren<IDamageable>();

        foreach (var d in damageables)
        {
            d.ReceiveDamage(context);
        }
    }
}