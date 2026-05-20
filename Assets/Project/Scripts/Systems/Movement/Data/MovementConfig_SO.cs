/*
 * Arquitectura: Movement/Data
 * Script: MovementConfig_SO
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona movimiento horizontal/vertical del jugador mediante estrategias y configuracion editable.
 * Relaciones: Recibe input desde PlayerInputHandler y expone contexto fisico usado por Jetpack.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "Game/Movement Config")]
public class MovementConfig_SO : ScriptableObject
{
    [Header("Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float smoothing = 10f;

    [Header("Vertical")]
    public float gravity = -20f;
    public float jumpForce = 8f;

    [Header("Jetpack")]
    public float jetpackForce = 12f;
    public float maxJetpackFuel  = 10f;
    public float jetpackBoostForce = 5f;
    public float maxJetpackHeight = 50f;

    [Header("Ground Detection")]
    public float groundedGraceTime = 0.15f;

}
