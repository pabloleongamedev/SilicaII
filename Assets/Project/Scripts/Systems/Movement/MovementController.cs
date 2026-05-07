using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementConfig_SO config;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
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
    private float groundedTimer;
    private bool inputEnabled = true;

    // fuerzas externas (abilities)
    private float externalVerticalForce;
    private Vector3 externalHorizontalForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        movementSystem = new MovementSystem();
        verticalSystem = new VerticalMovementSystem(config.gravity, config.jumpForce);
        jetpackSystem = new JetpackSystem(config.jetpackForce, config.maxJetpackFuel);

        abilitySystem = new AbilitySystem();
        jetpackAbility = new JetpackAbility(this, jetpackSystem);
        abilitySystem.Register(jetpackAbility);

        walkStrategy = new WalkMovement(config.walkSpeed);
        runStrategy = new RunMovement(config.runSpeed);

        movementSystem.SetStrategy(walkStrategy);

        rb.freezeRotation = true;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void SetSprint(bool isSprinting)
    {
        this.isSprinting = isSprinting;

        if (isGrounded)
            movementSystem.SetStrategy(isSprinting ? runStrategy : walkStrategy);
    }

    public void OnJumpStarted()
    {
        isJumpDown = true;
    }
    public void SetJetpack(bool isActive)
    {
        jetpackAbility.SetActive(isActive);
    }

    public void SetInputEnabled(bool value)
    {
        inputEnabled = value;
    }

    private void FixedUpdate()
    {
        CheckGround();

        // Strategy
        if (isGrounded)
            movementSystem.SetStrategy(isSprinting ? runStrategy : walkStrategy);
        else
            movementSystem.SetStrategy(walkStrategy);

        // Movimiento horizontal
        Vector3 desiredVelocity = movementSystem.CalculateVelocity(moveInput, transform);
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, config.smoothing * Time.fixedDeltaTime);

        // Movimiento vertical base
        verticalSystem.Tick(isGrounded, isJumpDown, Time.fixedDeltaTime);
        isJumpDown = false;

        // ABILITIES
        abilitySystem.Tick(Time.fixedDeltaTime);

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
        float rayDistance = config.groundCheckDistance;

        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, rayDistance))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);

            if (angle <= config.maxGroundAngle)
            {
                isGrounded = true;
                groundedTimer = config.groundedGraceTime;
                return;
            }
        }

        groundedTimer -= Time.fixedDeltaTime;
        isGrounded = groundedTimer > 0f;
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

    public float GetJetpackBoost()
    {
        return config.jetpackBoostForce;
    }

    public float GetHorizontalSpeed()
    {
        return new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
    }

    public float GetJetpackRatio()
    {
        return jetpackSystem.GetFuelRatio();
    }

    public float GetJetpackFuel()
    {
        return jetpackSystem.GetCurrentFuel();
    }

    public void RechargeJetpack(float amount)
    {
        jetpackSystem.Recharge(amount);
    }
    public float GetCurrentHeight()
    {
        return transform.position.y;
    }
}