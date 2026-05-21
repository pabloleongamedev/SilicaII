/*
 * Arquitectura: Player/Runtime
 * Script: PlayerAnimatorPresenter
 * Rol: Presenta animacion del modelo 3P leyendo MovementController, sin leer input ni mover al jugador.
 * Relaciones: MovementController publica estado real; Animator solo refleja Speed/Grounded/Jump/FreeFall/MotionSpeed.
 */
using UnityEngine;

public class PlayerAnimatorPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovementController movementController;
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float speedBlendRate = 10f;
    [SerializeField] private float airborneFreeFallDelay = 0.15f;

    private int speedId;
    private int groundedId;
    private int jumpId;
    private int freeFallId;
    private int motionSpeedId;
    private float currentSpeedBlend;
    private float airborneTimer;
    private bool hasLoggedMissingReferences;

    private void Awake()
    {
        ResolveReferences();
        CacheAnimatorIds();
    }

    private void Update()
    {
        if (!HasReferences())
            return;

        bool grounded = movementController.IsGrounded();
        Vector2 moveInput = movementController.GetMoveInput();
        float horizontalSpeed = movementController.GetHorizontalSpeed();
        float targetSpeed = moveInput.sqrMagnitude > 0.01f ? horizontalSpeed : 0f;

        currentSpeedBlend = Mathf.Lerp(currentSpeedBlend, targetSpeed, Time.deltaTime * speedBlendRate);
        if (currentSpeedBlend < 0.01f)
            currentSpeedBlend = 0f;

        airborneTimer = grounded ? 0f : airborneTimer + Time.deltaTime;

        animator.SetFloat(speedId, currentSpeedBlend);
        animator.SetFloat(motionSpeedId, moveInput.sqrMagnitude > 0.01f ? 1f : 0f);
        animator.SetBool(groundedId, grounded);
        animator.SetBool(jumpId, !grounded && airborneTimer <= airborneFreeFallDelay);
        animator.SetBool(freeFallId, !grounded && airborneTimer > airborneFreeFallDelay);
    }

    private void ResolveReferences()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if ((movementController == null || animator == null) && !hasLoggedMissingReferences)
        {
            hasLoggedMissingReferences = true;
            Debug.LogWarning("[PlayerAnimatorPresenter] Asigna MovementController y Animator por Inspector.", this);
        }
    }

    private bool HasReferences()
    {
        if (movementController != null && animator != null)
            return true;

        ResolveReferences();
        return movementController != null && animator != null;
    }

    private void CacheAnimatorIds()
    {
        speedId = Animator.StringToHash("Speed");
        groundedId = Animator.StringToHash("Grounded");
        jumpId = Animator.StringToHash("Jump");
        freeFallId = Animator.StringToHash("FreeFall");
        motionSpeedId = Animator.StringToHash("MotionSpeed");
    }
}
