/*
 * Arquitectura: Audio/Runtime
 * Script: PlayerFootstepAnimationEvents
 * Rol: Recibe AnimationEvents del modelo visual y delega sonidos de locomocion al feedback de audio del Player.
 * Relaciones: Vive junto al Animator que emite OnFootstep y OnLand.
 * Contrato AnimationEvent: las animaciones Walk/Run llaman OnFootstep; JumpLand llama OnLand.
 * Este script es la unica fuente de pasos/aterrizaje para audio y camara.
 */
using UnityEngine;
using System;

public class PlayerFootstepAnimationEvents : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAudioFeedback audioFeedback;

    private const float MinimumFootstepInterval = 0.12f;

    public event Action FootstepReceived;
    public event Action LandReceived;

    private float lastFootstepTime = -Mathf.Infinity;
    private bool hasLoggedMissingFeedback;

    public void OnFootstep()
    {
        if (Time.time - lastFootstepTime < MinimumFootstepInterval)
            return;

        lastFootstepTime = Time.time;
        FootstepReceived?.Invoke();

        if (!HasFeedback())
            return;

        audioFeedback.PlayFootstep();
    }

    public void OnLand()
    {
        LandReceived?.Invoke();

        if (!HasFeedback())
            return;

        audioFeedback.PlayLanding();
    }

    private bool HasFeedback()
    {
        if (audioFeedback != null)
            return true;

        if (!hasLoggedMissingFeedback)
        {
            hasLoggedMissingFeedback = true;
            Debug.LogWarning("[PlayerFootstepAnimationEvents] Asigna PlayerAudioFeedback por Inspector.", this);
        }

        return false;
    }
}
