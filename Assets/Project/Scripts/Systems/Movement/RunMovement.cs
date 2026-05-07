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