/*
 * Arquitectura: Movement/Runtime
 * Script: PlayerCameraMotion
 * Rol: Aplica movimiento visual de camara al caminar/correr leyendo MovementController.
 * Relaciones: PlayerFootstepAnimationEvents marca el pulso desde AnimationEvent OnFootstep.
 * Contrato simple: no calcula ritmo propio; si no llegan eventos de pasos, no mueve la camara.
 * PlayerPerspectiveController define el offset base y este script solo suma un offset temporal suave.
 */
using UnityEngine;

public class PlayerCameraMotion : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovementController movementController;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerFootstepAnimationEvents footstepEvents;

    [Header("Walk")]
    [SerializeField] private Vector3 walkStepAmplitude = new Vector3(0.08f, -0.14f, 0f);

    [Header("Run")]
    [SerializeField] private Vector3 runStepAmplitude = new Vector3(0.12f, -0.22f, 0f);

    [Header("Settings")]
    [SerializeField] private float minMoveInput = 0.05f;
    [SerializeField] private float stepDuration = 0.22f;
    [SerializeField] private float smoothTime = 0.06f;

    private Vector3 currentOffset;
    private Vector3 offsetVelocity;
    private Vector3 stepAmplitude;
    private float stepTimer;
    private int stepSide = 1;
    private bool hasLoggedMissingReferences;
    private bool hasLoggedMissingFootstepEvents;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void LateUpdate()
    {
        if (!HasReferences())
            return;

        Vector3 basePosition = cameraTransform.localPosition - currentOffset;
        Vector3 targetOffset = CalculateTargetOffset();

        currentOffset = Vector3.SmoothDamp(
            currentOffset,
            targetOffset,
            ref offsetVelocity,
            smoothTime);

        cameraTransform.localPosition = basePosition + currentOffset;
    }

    private void HandleFootstep()
    {
        if (!HasReferences() || !CanApplyMotion())
            return;

        Vector3 amplitude = movementController.IsSprinting() ? runStepAmplitude : walkStepAmplitude;
        stepAmplitude = new Vector3(amplitude.x * stepSide, amplitude.y, amplitude.z);
        stepTimer = Mathf.Max(0.01f, stepDuration);
        stepSide *= -1;
    }

    private Vector3 CalculateTargetOffset()
    {
        if (!CanApplyMotion() || stepTimer <= 0f)
        {
            stepTimer = 0f;
            return Vector3.zero;
        }

        stepTimer -= Time.deltaTime;

        float duration = Mathf.Max(0.01f, stepDuration);
        float normalizedTime = 1f - Mathf.Clamp01(stepTimer / duration);
        float pulse = Mathf.Sin(normalizedTime * Mathf.PI);
        return stepAmplitude * pulse;
    }

    private void ResolveReferences()
    {
        if ((movementController == null || cameraTransform == null) && !hasLoggedMissingReferences)
        {
            hasLoggedMissingReferences = true;
            Debug.LogWarning("[PlayerCameraMotion] Asigna MovementController y Camera Transform por Inspector.", this);
        }
    }

    private void Subscribe()
    {
        if (footstepEvents != null)
        {
            footstepEvents.FootstepReceived += HandleFootstep;
            return;
        }

        if (!hasLoggedMissingFootstepEvents)
        {
            hasLoggedMissingFootstepEvents = true;
            Debug.LogWarning("[PlayerCameraMotion] Asigna PlayerFootstepAnimationEvents por Inspector para sincronizar la camara con los pasos.", this);
        }
    }

    private void Unsubscribe()
    {
        if (footstepEvents != null)
            footstepEvents.FootstepReceived -= HandleFootstep;
    }

    private bool HasReferences()
    {
        if (movementController != null && cameraTransform != null)
            return true;

        ResolveReferences();
        return movementController != null && cameraTransform != null;
    }

    private bool CanApplyMotion()
    {
        Vector2 moveInput = movementController.GetMoveInput();
        return movementController.IsGrounded()
            && !movementController.IsJetpackActive()
            && moveInput.sqrMagnitude > minMoveInput * minMoveInput;
    }
}
