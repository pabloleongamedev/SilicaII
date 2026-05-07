public class WalkMovement : IMovementStrategy
{
    private float speed;

    public WalkMovement(float speed)
    {
        this.speed = speed;
    }

    public float GetSpeed()
    {
        return speed;
    }
}