/*
 * Arquitectura: Movement/Runtime
 * Script: MouseLook
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona movimiento horizontal/vertical del jugador mediante estrategias y configuracion editable.
 * Relaciones: Recibe input desde PlayerInputHandler y expone contexto fisico usado por Jetpack.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 2.5f;
    [SerializeField] private float smoothTime = 0.05f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;

    private Vector2 currentLook;
    private Vector2 currentLookVelocity;
    private Vector2 lookInput;

    private float pitch;

    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }

    private void LateUpdate()
    {
        // Suavizado
        currentLook = Vector2.SmoothDamp(
            currentLook,
            lookInput * sensitivity,
            ref currentLookVelocity,
            smoothTime
        );

        float mouseX = currentLook.x;
        float mouseY = currentLook.y;

        // Pitch (vertical)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Yaw (horizontal)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}