public class JetpackSystem
{
    private float jetpackForce;
    private float maxFuel;
    private float currentFuel;

    public JetpackSystem(float force, float maxTime)
    {
        this.jetpackForce = force;

        this.maxFuel = maxTime;
        this.currentFuel = maxTime;
    }

    public float Tick(bool isGrounded, float deltaTime)
    {
        if (isGrounded)
        {
            Recharge(deltaTime);
        }

        return 0f;
    }

    public float UseFuel(float deltaTime)
    {
        if (currentFuel <= 0f)
            return 0f;

        currentFuel -= deltaTime;

        if (currentFuel < 0f)
            currentFuel = 0f;

        return jetpackForce;
    }

    public void Recharge(float amount)
    {
        currentFuel += amount;

        if (currentFuel > maxFuel)
            currentFuel = maxFuel;
    }

    public float GetFuelRatio()
    {
        return currentFuel / maxFuel;
    }

    public float GetCurrentFuel()
    {
        return currentFuel;
    }
}