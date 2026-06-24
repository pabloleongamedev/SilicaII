/*
 * Arquitectura: Movement/Core
 * Script: MovementSystem
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona movimiento horizontal/vertical del jugador mediante estrategias y configuracion editable.
 * Relaciones: Recibe input desde PlayerInputHandler y expone contexto fisico usado por Jetpack.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class MovementSystem
{
    private IMovementStrategy currentStrategy;

    public void SetStrategy(IMovementStrategy strategy)
    {
        currentStrategy = strategy;
    }
       public IMovementStrategy GetStrategy()
    {
        return currentStrategy;
    }



    public Vector3 CalculateVelocity(Vector2 input, Vector3 forward, Vector3 right)
    {
        Vector3 direction =
            (forward * input.y) +
            (right * input.x);

        if (direction.magnitude > 1f)
            direction.Normalize();

        float speed = currentStrategy != null ? currentStrategy.GetSpeed() : 0f;

        return direction * speed;
    }
}
