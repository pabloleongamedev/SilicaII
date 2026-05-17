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

    private float startHeight;
    private bool hasStartedJetpack;
    private bool isActive;

    public JetpackAbility(IJetpackMovementContext movement, JetpackSystem jetpack)
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

        // Core rule: no recargar en el mismo tick en que el jugador esta consumiendo jetpack.
        // JetpackHUDPresenter lee GetFuelRatio desde IJetpackFuelReader, por eso el ratio debe reflejar consumo real.
        jetpack.Tick(isGrounded && !isActive, deltaTime);

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
                movement.GetForward() * movement.GetJetpackBoost() * deltaTime
            );
        }
    }
}
