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