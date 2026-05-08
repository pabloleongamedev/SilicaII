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
