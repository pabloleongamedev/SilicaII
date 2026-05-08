/*
 * Arquitectura: Health/Core
 * Script: DamageContext
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona dano, salud y componentes que pueden recibir dano.
 * Relaciones: Colabora con cualquier sistema que aplique dano mediante IDamageable y DamageDispatcher.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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