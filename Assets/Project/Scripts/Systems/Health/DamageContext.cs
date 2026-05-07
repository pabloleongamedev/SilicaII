using UnityEngine;
public struct DamageContext
{
    public int Amount;
    public Vector3 Point;
    public Vector3 Direction;

    public GameObject Source; // quien hizo el daño
    public object Metadata;   // opcional (ej: tipo de daño)

    public DamageContext(int amount, Vector3 point, Vector3 direction, GameObject source, object metadata = null)
    {
        Amount = amount;
        Point = point;
        Direction = direction;
        Source = source;
        Metadata = metadata;
    }
}