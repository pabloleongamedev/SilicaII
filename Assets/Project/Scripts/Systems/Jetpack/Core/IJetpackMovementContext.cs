/*
 * Arquitectura: Jetpack/Core
 * Script: IJetpackMovementContext
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona combustible, empuje y habilidades asociadas al propulsor, colaborando con Movement mediante una interfaz.
 * Relaciones: Usa IJetpackMovementContext para aplicar fuerzas sin depender de MovementController concreto.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public interface IJetpackMovementContext
{
    bool IsGrounded();
    bool IsSprinting();
    float GetCurrentHeight();
    float GetMaxJetpackHeight();
    float GetJetpackBoost();
    Vector3 GetForward();
    void AddExternalVerticalForce(float force);
    void AddExternalHorizontalForce(Vector3 force);
}
