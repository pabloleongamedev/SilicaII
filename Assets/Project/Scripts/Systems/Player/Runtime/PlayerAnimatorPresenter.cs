/*
 * Arquitectura: Player/Runtime
 * Script: PlayerAnimatorPresenter
 * Rol: Presenta animacion del modelo 3P leyendo MovementController, sin leer input ni mover al jugador.
 * Relaciones: MovementController publica estado real; Animator solo refleja Speed/Grounded/Jump/FreeFall/MotionSpeed.
 * Contrato Animator: requiere parametros Speed, Grounded, Jump, FreeFall y MotionSpeed.
 * Jetpack: mientras hay jetpack efectivo, fuerza locomocion idle y solo inclina el visual.
 */
using UnityEngine;

public class PlayerAnimatorPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovementController movementController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualTiltRoot;

    [Header("Settings")]
    [SerializeField] private float speedBlendRate = 10f;
    [SerializeField] private float airborneFreeFallDelay = 0.15f;

    private const string SpeedParameter = "Speed";
    private const string GroundedParameter = "Grounded";
    private const string JumpParameter = "Jump";
    private const string FreeFallParameter = "FreeFall";
    private const string MotionSpeedParameter = "MotionSpeed";
    private const string LocomotionStateName = "Idle Walk Run Blend";
    private const float JumpVerticalSpeedThreshold = 0.25f;
    private const float FallVerticalSpeedThreshold = -0.25f;
    private const float JetpackTiltAngle = 20f;
    private const float JetpackBoostTiltBonus = 10f;
    private const float JetpackTiltSmooth = 10f;

    private int speedId;
    private int groundedId;
    private int jumpId;
    private int freeFallId;
    private int motionSpeedId;
    private float currentSpeedBlend;
    private float airborneTimer;
    private Quaternion baseVisualLocalRotation = Quaternion.identity;
    private bool wasEffectiveJetpackActive;
    private bool hasLoggedMissingReferences;
    private bool hasLoggedMissingVisualTiltRoot;
    private bool hasLoggedMissingAnimatorController;
    private bool hasSpeedParameter;
    private bool hasGroundedParameter;
    private bool hasJumpParameter;
    private bool hasFreeFallParameter;
    private bool hasMotionSpeedParameter;
    private bool hasLocomotionState;

    private void Awake()
    {
        ResolveReferences();
        CacheAnimatorIds();
        ValidateAnimatorContract();
        CacheVisualPose();
    }

    private void Update()
    {
        if (!HasReferences())
            return;

        bool grounded = movementController.IsGrounded();
        Vector2 moveInput = movementController.GetMoveInput();
        float horizontalSpeed = movementController.GetHorizontalSpeed();
        float verticalSpeed = movementController.GetVerticalSpeed();
        bool hasMoveInput = moveInput.sqrMagnitude > 0.01f;
        bool effectiveJetpackActive = IsEffectiveJetpackActive(grounded);
        bool boostActive = effectiveJetpackActive && movementController.IsSprinting();
        bool jumping = !effectiveJetpackActive && verticalSpeed >= JumpVerticalSpeedThreshold;
        float targetSpeed = !effectiveJetpackActive && hasMoveInput ? horizontalSpeed : 0f;

        currentSpeedBlend = Mathf.Lerp(currentSpeedBlend, targetSpeed, Time.deltaTime * speedBlendRate);
        if (currentSpeedBlend < 0.01f)
            currentSpeedBlend = 0f;

        airborneTimer = grounded ? 0f : airborneTimer + Time.deltaTime;
        bool falling = !effectiveJetpackActive
            && !grounded
            && verticalSpeed <= FallVerticalSpeedThreshold
            && airborneTimer > airborneFreeFallDelay;

        if (effectiveJetpackActive && !wasEffectiveJetpackActive && hasLocomotionState)
            animator.CrossFade(LocomotionStateName, 0.08f);

        if (hasSpeedParameter)
            animator.SetFloat(speedId, currentSpeedBlend);

        if (hasMotionSpeedParameter)
            animator.SetFloat(motionSpeedId, !effectiveJetpackActive && hasMoveInput ? 1f : 0f);

        if (hasGroundedParameter)
            animator.SetBool(groundedId, grounded);

        if (hasJumpParameter)
            animator.SetBool(jumpId, jumping);

        if (hasFreeFallParameter)
            animator.SetBool(freeFallId, falling);

        ApplyJetpackTilt(effectiveJetpackActive, boostActive, moveInput);
        wasEffectiveJetpackActive = effectiveJetpackActive;
    }

    private void ResolveReferences()
    {
        if ((movementController == null || animator == null) && !hasLoggedMissingReferences)
        {
            hasLoggedMissingReferences = true;
            Debug.LogWarning("[PlayerAnimatorPresenter] Asigna MovementController y Animator por Inspector.", this);
        }
    }

    private bool HasReferences()
    {
        if (movementController == null || animator == null)
        {
            ResolveReferences();
            return false;
        }

        if (animator.runtimeAnimatorController != null)
            return true;

        if (!hasLoggedMissingAnimatorController)
        {
            hasLoggedMissingAnimatorController = true;
            Debug.LogWarning("[PlayerAnimatorPresenter] El Animator asignado no tiene RuntimeAnimatorController.", this);
        }

        return false;
    }

    private void CacheVisualPose()
    {
        if (visualTiltRoot != null)
            baseVisualLocalRotation = visualTiltRoot.localRotation;
    }

    private bool IsEffectiveJetpackActive(bool grounded)
    {
        return !grounded
            && movementController.IsJetpackActive()
            && movementController.GetCurrentFuel() > 0f;
    }

    private void ApplyJetpackTilt(bool jetpackActive, bool boostActive, Vector2 moveInput)
    {
        if (visualTiltRoot == null)
        {
            if (jetpackActive && !hasLoggedMissingVisualTiltRoot)
            {
                hasLoggedMissingVisualTiltRoot = true;
                Debug.LogWarning("[PlayerAnimatorPresenter] Asigna Visual Tilt Root por Inspector para inclinar el visual durante jetpack.", this);
            }

            return;
        }

        Quaternion targetRotation = baseVisualLocalRotation;
        if (jetpackActive && moveInput.sqrMagnitude > 0.01f)
        {
            Vector2 direction = Vector2.ClampMagnitude(moveInput, 1f);
            float angle = JetpackTiltAngle + (boostActive ? JetpackBoostTiltBonus : 0f);
            Quaternion tiltRotation = Quaternion.Euler(direction.y * angle, 0f, -direction.x * angle);
            targetRotation = baseVisualLocalRotation * tiltRotation;
        }

        visualTiltRoot.localRotation = Quaternion.Slerp(
            visualTiltRoot.localRotation,
            targetRotation,
            Time.deltaTime * JetpackTiltSmooth);
    }

    private void CacheAnimatorIds()
    {
        speedId = Animator.StringToHash(SpeedParameter);
        groundedId = Animator.StringToHash(GroundedParameter);
        jumpId = Animator.StringToHash(JumpParameter);
        freeFallId = Animator.StringToHash(FreeFallParameter);
        motionSpeedId = Animator.StringToHash(MotionSpeedParameter);
    }

    private void ValidateAnimatorContract()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        hasSpeedParameter = HasAnimatorParameter(SpeedParameter, AnimatorControllerParameterType.Float);
        hasGroundedParameter = HasAnimatorParameter(GroundedParameter, AnimatorControllerParameterType.Bool);
        hasJumpParameter = HasAnimatorParameter(JumpParameter, AnimatorControllerParameterType.Bool);
        hasFreeFallParameter = HasAnimatorParameter(FreeFallParameter, AnimatorControllerParameterType.Bool);
        hasMotionSpeedParameter = HasAnimatorParameter(MotionSpeedParameter, AnimatorControllerParameterType.Float);
        hasLocomotionState = animator.HasState(0, Animator.StringToHash(LocomotionStateName));

        if (!hasLocomotionState)
            Debug.LogWarning($"[PlayerAnimatorPresenter] El Animator no contiene el estado '{LocomotionStateName}' en layer 0; jetpack no forzara CrossFade a locomocion.", this);
    }

    private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType expectedType)
    {
        foreach (var parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == expectedType)
                return true;
        }

        Debug.LogWarning($"[PlayerAnimatorPresenter] Falta parametro Animator '{parameterName}' de tipo {expectedType}.", this);
        return false;
    }
}
