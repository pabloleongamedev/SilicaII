/*
 * Arquitectura: Health/Core
 * Script: IDamageable
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona dano, salud y componentes que pueden recibir dano.
 * Relaciones: Colabora con cualquier sistema que aplique dano mediante IDamageable y DamageDispatcher.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
public interface IDamageable
{
    void ReceiveDamage(in DamageContext context);
}