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