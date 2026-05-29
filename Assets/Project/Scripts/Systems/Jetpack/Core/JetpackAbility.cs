/*
 * Arquitectura: Jetpack/Core
 * Script: JetpackAbility
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona combustible, empuje y habilidades asociadas al propulsor, colaborando con Movement mediante una interfaz.
 * Relaciones: Usa IJetpackMovementContext para aplicar fuerzas sin depender de MovementController concreto.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
public class JetpackAbility : IAbility
{
    private IJetpackMovementContext movement;
    private JetpackSystem jetpack;
    private float rampUpTime;

    private float startHeight;
    private float activeTime;
    private bool hasStartedJetpack;
    private bool isActive;

    public JetpackAbility(IJetpackMovementContext movement, JetpackSystem jetpack, float rampUpTime)
    {
        this.movement = movement;
        this.jetpack = jetpack;
        this.rampUpTime = rampUpTime;
    }

    public void SetActive(bool active)
    {
        isActive = active;

        // 🔥 reset limpio al soltar botón
        if (!active)
        {
            activeTime = 0f;
            hasStartedJetpack = false;
        }
    }

    public void Tick(float deltaTime)
    {
        bool isGrounded = movement.IsGrounded();

        // Core rule: no recargar en el mismo tick en que el jugador esta consumiendo jetpack.
        // JetpackHUDPresenter lee GetFuelRatio desde IJetpackFuelReader, por eso el ratio debe reflejar consumo real.
        jetpack.Tick(isGrounded && !isActive, deltaTime);

        if (!isActive)
            return;

        // 🔥 guardar altura SOLO cuando se activa
        if (!hasStartedJetpack)
        {
            startHeight = movement.GetCurrentHeight();
            activeTime = 0f;
            hasStartedJetpack = true;
        }

        activeTime += deltaTime;
        float currentHeight = movement.GetCurrentHeight();
        float heightDelta = currentHeight - startHeight;

        // 🔥 LIMITE REAL
        if (heightDelta >= movement.GetMaxJetpackHeight())
            return;

        float force = jetpack.UseFuel(deltaTime);

        if (force <= 0f)
            return;

        float power = GetRampPower();

        movement.AddExternalVerticalForce(force * power);

        // boost horizontal
        if (!isGrounded && movement.IsSprinting())
        {
            movement.AddExternalHorizontalForce(
                movement.GetForward() * movement.GetJetpackBoost() * power * deltaTime
            );
        }
    }

    private float GetRampPower()
    {
        if (rampUpTime <= 0f)
            return 1f;

        return UnityEngine.Mathf.Clamp01(activeTime / rampUpTime);
    }
}
