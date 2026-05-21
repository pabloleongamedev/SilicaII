/*
 * Arquitectura: Player/Runtime
 * Script: PlayerPerspectiveController
 * Rol: Alterna presentacion primera/tercera persona desde input centralizado.
 * Relaciones: PlayerInputHandler invoca TogglePerspective; las referencias visuales/camaras se asignan por Inspector.
 */
using UnityEngine;

public class PlayerPerspectiveController : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private PlayerPerspectiveMode initialMode = PlayerPerspectiveMode.FirstPerson;

    [Header("First Person")]
    [SerializeField] private GameObject firstPersonCameraRoot;
    [SerializeField] private GameObject firstPersonVisualRoot;

    [Header("Third Person")]
    [SerializeField] private GameObject thirdPersonCameraRoot;
    [SerializeField] private GameObject thirdPersonVisualRoot;

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
        SetActive(firstPersonCameraRoot, !thirdPerson);
        SetActive(firstPersonVisualRoot, !thirdPerson);
        SetActive(thirdPersonCameraRoot, thirdPerson);
        SetActive(thirdPersonVisualRoot, thirdPerson);
    }

    private void SetActive(GameObject target, bool active)
    {
        if (target == null)
        {
            Debug.LogWarning("[PlayerPerspectiveController] Asigna todos los roots de perspectiva por Inspector.", this);
            return;
        }

        if (target.activeSelf != active)
            target.SetActive(active);
    }
}
