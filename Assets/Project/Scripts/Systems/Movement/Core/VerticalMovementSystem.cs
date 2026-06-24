/*
 * Arquitectura: Movement/Core
 * Script: VerticalMovementSystem
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona movimiento horizontal/vertical del jugador mediante estrategias y configuracion editable.
 * Relaciones: Recibe input desde PlayerInputHandler y expone contexto fisico usado por Jetpack.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class VerticalMovementSystem
{
    private float verticalVelocity;

    private float gravity;
    private float jumpForce;

    public VerticalMovementSystem(float gravity, float jumpForce)
    {
        this.gravity = gravity;
        this.jumpForce = jumpForce;
    }

    public void Tick(bool isGrounded, bool jumpDown, float deltaTime)
    {
        if (isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f;

            if (jumpDown)
                verticalVelocity = jumpForce;
        }
        else
        {
            verticalVelocity += gravity * deltaTime;

            // limitar la caída
            float maxFallSpeed = -20f;

            if (verticalVelocity < maxFallSpeed)
                verticalVelocity = maxFallSpeed;
        }
    }
    public float GetVelocity()
    {
        return verticalVelocity;
    }
}