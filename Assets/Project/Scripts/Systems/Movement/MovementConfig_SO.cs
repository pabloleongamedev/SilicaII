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
    public float groundCheckDistance = 0.4f;
    public float maxGroundAngle = 60f;
    public float groundedGraceTime = 0.15f;

}