/*
 * Arquitectura: Player/Runtime
 * Script: PlayerPerspectiveController
 * Rol: Alterna presentacion primera/tercera persona desde input centralizado.
 * Relaciones: PlayerInputHandler invoca TogglePerspective; la camara compartida se asigna por Inspector.
 */
using UnityEngine;

public class PlayerPerspectiveController : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private PlayerPerspectiveMode initialMode = PlayerPerspectiveMode.FirstPerson;

    [Header("Shared Camera Offset")]
    [SerializeField] private Transform sharedCameraTransform;
    [SerializeField] private Vector3 firstPersonLocalPosition;
    [SerializeField] private Vector3 firstPersonLocalEulerAngles;
    [SerializeField] private Vector3 thirdPersonLocalPosition = new Vector3(0f, 0.35f, -3f);
    [SerializeField] private Vector3 thirdPersonLocalEulerAngles;

    private PlayerPerspectiveMode currentMode;

    public PlayerPerspectiveMode CurrentMode => currentMode;

    private void Awake()
    {
        ApplyMode(initialMode, true);
    }

    public void TogglePerspective()
    {
        ApplyMode(currentMode == PlayerPerspectiveMode.FirstPerson
            ? PlayerPerspectiveMode.ThirdPerson
            : PlayerPerspectiveMode.FirstPerson);
    }

    public void SetPerspective(PlayerPerspectiveMode mode)
    {
        ApplyMode(mode);
    }

    private void ApplyMode(PlayerPerspectiveMode mode, bool force = false)
    {
        if (!force && currentMode == mode)
            return;

        currentMode = mode;

        bool thirdPerson = currentMode == PlayerPerspectiveMode.ThirdPerson;
        ApplySharedCameraOffset(thirdPerson);
    }

    private void ApplySharedCameraOffset(bool thirdPerson)
    {
        if (sharedCameraTransform == null)
        {
            Debug.LogWarning("[PlayerPerspectiveController] Asigna Shared Camera Transform por Inspector.", this);
            return;
        }

        if (firstPersonLocalPosition == thirdPersonLocalPosition && firstPersonLocalEulerAngles == thirdPersonLocalEulerAngles)
            Debug.LogWarning("[PlayerPerspectiveController] Los offsets de primera y tercera persona son iguales; el cambio de perspectiva no sera visible.", this);

        sharedCameraTransform.localPosition = thirdPerson ? thirdPersonLocalPosition : firstPersonLocalPosition;
        sharedCameraTransform.localRotation = Quaternion.Euler(thirdPerson ? thirdPersonLocalEulerAngles : firstPersonLocalEulerAngles);
    }
}
