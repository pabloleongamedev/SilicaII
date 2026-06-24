/*
 * Arquitectura: Movement/Runtime
 * Script: PlayerCameraRig
 * Rol: Controla la vista FPS del jugador desde el propio GameObject Player.
 * Relaciones: PlayerInputHandler envia el delta de mouse; MovementController usa el forward/right del Player ya rotado.
 * Uso como referencia: jerarquia esperada Player -> CameraPivot -> Main Camera. El input actualiza
 * una rotacion objetivo y el suavizado interpola la vista sin deformar el delta crudo del mouse.
 */
using UnityEngine;

public class PlayerCameraRig : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform pitchPivot;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 0.3f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;
    [SerializeField] private bool smoothLook = true;
    [SerializeField] private float lookSmoothTime = 0.035f;

    private Vector2 pendingLookDelta;
    private float yaw;
    private float pitch;
    private float targetYaw;
    private float targetPitch;
    private float yawVelocity;
    private float pitchVelocity;

    private void Awake()
    {
        if (pitchPivot == null && transform.childCount > 0)
            pitchPivot = transform.GetChild(0);

        targetYaw = transform.eulerAngles.y;
        targetPitch = pitchPivot != null ? NormalizePitch(pitchPivot.localEulerAngles.x) : 0f;
        yaw = targetYaw;
        pitch = targetPitch;
    }

    private void OnEnable()
    {
        pendingLookDelta = Vector2.zero;
        yawVelocity = 0f;
        pitchVelocity = 0f;
        ApplyRotation();
    }

    public void SetLookInput(Vector2 input)
    {
        pendingLookDelta += input;
    }

    private void Update()
    {
        ApplyLookInput();
        UpdateSmoothedRotation();
    }

    private void ApplyLookInput()
    {
        if (pendingLookDelta == Vector2.zero)
            return;

        Vector2 lookDelta = pendingLookDelta * sensitivity;
        pendingLookDelta = Vector2.zero;

        targetYaw += lookDelta.x;
        targetPitch -= lookDelta.y;
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
    }

    private void UpdateSmoothedRotation()
    {
        if (!smoothLook || lookSmoothTime <= 0f)
        {
            yaw = targetYaw;
            pitch = targetPitch;
            ApplyRotation();
            return;
        }

        yaw = Mathf.SmoothDampAngle(yaw, targetYaw, ref yawVelocity, lookSmoothTime);
        pitch = Mathf.SmoothDampAngle(pitch, targetPitch, ref pitchVelocity, lookSmoothTime);
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);

        // Yaw en el Player. MovementController lee este forward/right en FixedUpdate.
        transform.rotation = yawRotation;

        // Pitch solo en el pivote para evitar inclinar el cuerpo/collider.
        if (pitchPivot != null)
            pitchPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private float NormalizePitch(float eulerX)
    {
        return eulerX > 180f ? eulerX - 360f : eulerX;
    }
}
