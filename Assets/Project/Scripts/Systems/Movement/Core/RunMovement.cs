/*
 * Arquitectura: Movement/Core
 * Script: RunMovement
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona movimiento horizontal/vertical del jugador mediante estrategias y configuracion editable.
 * Relaciones: Recibe input desde PlayerInputHandler y expone contexto fisico usado por Jetpack.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
public class RunMovement : IMovementStrategy
{
    private float speed;

    public RunMovement(float speed)
    {
        this.speed = speed;
    }

    public float GetSpeed()
    {
        return speed;
    }
}