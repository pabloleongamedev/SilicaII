/*
 * Arquitectura: Movement/Runtime
 * Script: MovementController
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona movimiento horizontal/vertical del jugador mediante estrategias y configuracion editable.
 * Relaciones: Recibe input desde PlayerInputHandler, expone contexto fisico usado por Jetpack e implementa IJetpackFuelReader para HUD/SaveLoad.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour, IJetpackMovementContext, IJetpackFuelReader
{
    [SerializeField] private MovementConfig_SO config;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;

    [Header("Rigidbody Presentation")]
    [SerializeField] private bool useRigidbodyInterpolation = true;

    [Header("Movement Events")]
    [SerializeField] private float minLandingHeightDelta = 0.2f;

    private Rigidbody rb;

    private MovementSystem movementSystem;
    private VerticalMovementSystem verticalSystem;
    private JetpackSystem jetpackSystem;
    private AbilitySystem abilitySystem;
    private JetpackAbility jetpackAbility;
    private IMovementStrategy walkStrategy;
    private IMovementStrategy runStrategy;
    private Vector2 moveInput;
    private Vector3 currentVelocity;

    private bool isGrounded;
    private bool isJumpDown;
    private bool isSprinting;
    private bool isJetpackActive;
    private bool isJetpackConsumingFuel;
    private float groundedTimer;
    private float lastGroundedCheckHeight;
    private float maxAirborneCheckHeight;
    private bool inputEnabled = true;
    private float lastPublishedFuelRatio = -1f;
    private string currentGroundTag = "Ground";
    private bool hasGroundStateInitialized;
    private readonly Collider[] groundHits = new Collider[8];

    public event System.Action<float> OnFuelRatioChanged;
    public event System.Action<Vector2> OnMoveInputChanged;
    public event System.Action<bool> OnSprintChanged;
    public event System.Action<string> OnLanded;
    public event System.Action<bool> OnJetpackActiveChanged;
    public event System.Action<bool, string> OnGroundStateChanged;

    // fuerzas externas (abilities)
    private float externalVerticalForce;
    private Vector3 externalHorizontalForce;

    private void Awake()
    {
        // Composicion Runtime: crea sistemas Core puros y los conecta con Rigidbody.
        // MovementController es el puente Unity/physics; no deberia contener UI.
        rb = GetComponent<Rigidbody>();
        ConfigureRigidbodyPresentation();

        movementSystem = new MovementSystem();
        verticalSystem = new VerticalMovementSystem(config.gravity, config.jumpForce);
        jetpackSystem = new JetpackSystem(config.jetpackForce, config.maxJetpackFuel);

        abilitySystem = new AbilitySystem();
        jetpackAbility = new JetpackAbility(this, jetpackSystem, config.jetpackRampUpTime);
        abilitySystem.Register(jetpackAbility);

        walkStrategy = new WalkMovement(config.walkSpeed);
        runStrategy = new RunMovement(config.runSpeed);

        movementSystem.SetStrategy(walkStrategy);

        ConfigureRigidbodyConstraints();
    }

    private void ConfigureRigidbodyPresentation()
    {
        // La camara vive bajo el Player y renderiza en el frame visual, mientras el movimiento fisico corre en FixedUpdate.
        // Interpolate suaviza la posicion presentada entre ticks fisicos y evita tirones visibles al sprintar.
        if (useRigidbodyInterpolation)
            rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void ConfigureRigidbodyConstraints()
    {
        // La camara controla el yaw visual del Player en Update. La fisica no debe
        // corregir rotacion, porque eso genera micro saltos entre Update y FixedUpdate.
        rb.freezeRotation = true;
    }

    public void SetMoveInput(Vector2 input)
    {
        if (moveInput == input)
            return;

        moveInput = input;
        OnMoveInputChanged?.Invoke(moveInput);
    }

    public void SetSprint(bool isSprinting)
    {
        if (this.isSprinting == isSprinting)
            return;

        this.isSprinting = isSprinting;
        OnSprintChanged?.Invoke(this.isSprinting);

        if (isGrounded)
            movementSystem.SetStrategy(isSprinting ? runStrategy : walkStrategy);
    }

    public void OnJumpStarted()
    {
        isJumpDown = true;
    }
    public void SetJetpack(bool isActive)
    {
        if (isJetpackActive == isActive)
            return;

        isJetpackActive = isActive;
        jetpackAbility.SetActive(isActive);
        OnJetpackActiveChanged?.Invoke(isJetpackActive);
    }

    public void SetInputEnabled(bool value)
    {
        inputEnabled = value;
    }

    private void FixedUpdate()
    {
        // Pipeline fisico: suelo -> estrategia walk/run -> velocidad horizontal ->
        // vertical/jump -> abilities -> velocidad final del Rigidbody.
        CheckGround();

        // Strategy
        if (isGrounded)
            movementSystem.SetStrategy(isSprinting ? runStrategy : walkStrategy);
        else
            movementSystem.SetStrategy(walkStrategy);

        // Movimiento horizontal
        Vector3 desiredVelocity = movementSystem.CalculateVelocity(moveInput, transform.forward, transform.right);
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, config.smoothing * Time.fixedDeltaTime);

        // Movimiento vertical base
        verticalSystem.Tick(isGrounded, isJumpDown, Time.fixedDeltaTime);
        isJumpDown = false;

        // ABILITIES
        float fuelBeforeAbilities = GetCurrentFuel();
        abilitySystem.Tick(Time.fixedDeltaTime);
        isJetpackConsumingFuel = isJetpackActive && GetCurrentFuel() < fuelBeforeAbilities;
        PublishFuelRatioIfChanged();

        // aplicar fuerzas finales
        Vector3 finalVelocity = currentVelocity + externalHorizontalForce;
        float finalY = verticalSystem.GetVelocity() + externalVerticalForce;

        rb.linearVelocity = new Vector3(finalVelocity.x, finalY, finalVelocity.z);

        // reset fuerzas externas
        externalVerticalForce = 0f;
        externalHorizontalForce = Vector3.zero;
    }
    void Update()
    {
        if (!inputEnabled)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }
    }

    private void CheckGround()
    {
        Vector3 checkPosition = groundCheck != null ? groundCheck.position : transform.position;
        int hitCount = Physics.OverlapSphereNonAlloc(
            checkPosition,
            groundCheckRadius,
            groundHits,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = groundHits[i];

            if (hit == null || hit.attachedRigidbody == rb || hit.transform.IsChildOf(transform))
                continue;

            groundedTimer = config.groundedGraceTime;
            UpdateGroundState(true, hit.tag);
            return;
        }

        groundedTimer -= Time.fixedDeltaTime;
        bool hasGroundGrace = groundedTimer > 0f;
        UpdateGroundState(hasGroundGrace, hasGroundGrace ? currentGroundTag : "Ground");
    }

    private void UpdateGroundState(bool grounded, string groundTag)
    {
        float currentGroundCheckHeight = GetGroundCheckHeight();

        if (!hasGroundStateInitialized)
        {
            hasGroundStateInitialized = true;
            isGrounded = grounded;
            currentGroundTag = groundTag;
            lastGroundedCheckHeight = currentGroundCheckHeight;
            maxAirborneCheckHeight = currentGroundCheckHeight;
            OnGroundStateChanged?.Invoke(isGrounded, currentGroundTag);
            return;
        }

        if (isGrounded == grounded && currentGroundTag == groundTag)
        {
            if (!isGrounded)
                maxAirborneCheckHeight = Mathf.Max(maxAirborneCheckHeight, currentGroundCheckHeight);

            return;
        }

        bool wasGrounded = isGrounded;

        if (wasGrounded && !grounded)
        {
            lastGroundedCheckHeight = currentGroundCheckHeight;
            maxAirborneCheckHeight = currentGroundCheckHeight;
        }

        if (!wasGrounded && !grounded)
        {
            maxAirborneCheckHeight = Mathf.Max(maxAirborneCheckHeight, currentGroundCheckHeight);
        }

        isGrounded = grounded;
        currentGroundTag = groundTag;
        OnGroundStateChanged?.Invoke(isGrounded, currentGroundTag);

        if (!wasGrounded && isGrounded && HasMeaningfulAirborneHeight())
            OnLanded?.Invoke(currentGroundTag);

        if (isGrounded)
        {
            lastGroundedCheckHeight = currentGroundCheckHeight;
            maxAirborneCheckHeight = currentGroundCheckHeight;
        }
    }

    private bool HasMeaningfulAirborneHeight()
    {
        return maxAirborneCheckHeight - lastGroundedCheckHeight >= minLandingHeightDelta;
    }

    private float GetGroundCheckHeight()
    {
        return groundCheck != null ? groundCheck.position.y : transform.position.y;
    }
    public float GetMaxJetpackHeight()
    {
        return config.maxJetpackHeight;
    }

    // ===== API PARA ABILITIES =====

    public void AddExternalVerticalForce(float force)
    {
        externalVerticalForce += force;
    }

    public void AddExternalHorizontalForce(Vector3 force)
    {
        externalHorizontalForce += force;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public bool IsJetpackActive()
    {
        return isJetpackActive;
    }

    public bool IsJetpackConsumingFuel()
    {
        return isJetpackConsumingFuel;
    }

    public string GetGroundTag()
    {
        return currentGroundTag;
    }

    public float GetJetpackBoost()
    {
        return config.jetpackBoostForce;
    }

    public float GetHorizontalSpeed()
    {
        return new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
    }

    public float GetVerticalSpeed()
    {
        return rb.linearVelocity.y;
    }

    public float GetJetpackRatio()
    {
        return GetFuelRatio();
    }

    public float GetFuelRatio()
    {
        // API segura para UI: los presenters consumen IJetpackFuelReader sin conocer JetpackSystem.
        if (jetpackSystem == null)
            return 0f;

        return jetpackSystem.GetFuelRatio();
    }

    public float GetJetpackFuel()
    {
        return GetCurrentFuel();
    }

    public float GetCurrentFuel()
    {
        if (jetpackSystem == null)
            return 0f;

        return jetpackSystem.GetCurrentFuel();
    }

    public void RechargeJetpack(float amount)
    {
        if (jetpackSystem == null)
            return;

        jetpackSystem.Recharge(amount);
        PublishFuelRatioIfChanged(true);
    }

    public void RestoreJetpackFuel(float amount)
    {
        if (jetpackSystem == null)
            return;

        jetpackSystem.RestoreFuel(amount);
        PublishFuelRatioIfChanged(true);
    }

    public float GetCurrentHeight()
    {
        return transform.position.y;
    }

    public Vector3 GetForward()
    {
        return transform.forward;
    }

    private void PublishFuelRatioIfChanged(bool force = false)
    {
        float ratio = GetFuelRatio();

        if (!force && Mathf.Approximately(ratio, lastPublishedFuelRatio))
            return;

        lastPublishedFuelRatio = ratio;
        OnFuelRatioChanged?.Invoke(ratio);
    }
}
