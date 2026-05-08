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
