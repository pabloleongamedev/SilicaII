/*
 * Arquitectura: Audio/Runtime
 * Script: PlayerFootstepAnimationEvents
 * Rol: Recibe AnimationEvents del modelo visual y delega sonidos de locomocion al feedback de audio del Player.
 * Relaciones: Vive junto al Animator que emite OnFootstep; PlayerAudioFeedback decide superficie, pitch y servicio de audio.
 */
using UnityEngine;

public class PlayerFootstepAnimationEvents : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAudioFeedback audioFeedback;

    private bool hasLoggedMissingFeedback;

    public void OnFootstep()
    {
        if (!HasFeedback())
            return;

        audioFeedback.PlayFootstep();
    }

    public void OnLand()
    {
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
