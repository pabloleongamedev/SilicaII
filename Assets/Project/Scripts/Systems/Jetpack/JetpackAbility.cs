public class JetpackAbility : IAbility
{
    private MovementController movement;
    private JetpackSystem jetpack;

    private float startHeight;
    private bool hasStartedJetpack;
    private bool isActive;

    public JetpackAbility(MovementController movement, JetpackSystem jetpack)
    {
        this.movement = movement;
        this.jetpack = jetpack;
    }

    public void SetActive(bool active)
    {
        isActive = active;

        // 🔥 reset limpio al soltar botón
        if (!active)
        {
            hasStartedJetpack = false;
        }
    }

    public void Tick(float deltaTime)
    {
        bool isGrounded = movement.IsGrounded();

        // recarga siempre
        jetpack.Tick(isGrounded, deltaTime);

        if (!isActive)
            return;

        // 🔥 guardar altura SOLO cuando se activa
        if (!hasStartedJetpack)
        {
            startHeight = movement.GetCurrentHeight();
            hasStartedJetpack = true;
        }

        float currentHeight = movement.GetCurrentHeight();
        float heightDelta = currentHeight - startHeight;

        // 🔥 LIMITE REAL
        if (heightDelta >= movement.GetMaxJetpackHeight())
            return;

        float force = jetpack.UseFuel(deltaTime);

        if (force <= 0f)
            return;

        movement.AddExternalVerticalForce(force);

        // boost horizontal
        if (!isGrounded && movement.IsSprinting())
        {
            movement.AddExternalHorizontalForce(
                movement.transform.forward * movement.GetJetpackBoost() * deltaTime
            );
        }
    }
}