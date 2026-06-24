/*
 * Arquitectura: Audio/Runtime
 * Script: PlayerAudioFeedback
 * Rol: Feedback audiovisual del Player. Observa MovementController y solicita AudioCueKey al IAudioService central.
 * Relaciones: PlayerFootstepAnimationEvents invoca PlayFootstep/PlayLanding; MovementController publica estado de jetpack y superficie.
 * Uso como referencia: mantiene audio desacoplado de Input System y evita duplicar reglas de movimiento.
 */
using UnityEngine;

public class PlayerAudioFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovementController movementController;
    [SerializeField] private MonoBehaviour audioServiceBehaviour;

    [Header("Settings")]
    [SerializeField] private float walkPitch = 1f;
    [SerializeField] private float sprintPitch = 1.5f;

    private IAudioService audioService;
    private bool isGrounded;
    private bool isSprinting;
    private bool isJetpackActive;
    private bool isJetpackAudioPlaying;
    private string currentGroundTag = "Ground";
    private bool hasLoggedMissingAudioService;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        ResolveReferences();
        Subscribe();
        SyncFromMovement();
        RefreshJetpackAudio();
    }

    private void OnDisable()
    {
        Unsubscribe();
        StopJetpackAudio();
    }

    private void Update()
    {
        SyncFromMovement();
        RefreshJetpackAudio();
    }

    private void Subscribe()
    {
        if (movementController == null)
            return;

        movementController.OnSprintChanged += HandleSprintChanged;
        movementController.OnJetpackActiveChanged += HandleJetpackActiveChanged;
        movementController.OnGroundStateChanged += HandleGroundStateChanged;
    }

    private void Unsubscribe()
    {
        if (movementController == null)
            return;

        movementController.OnSprintChanged -= HandleSprintChanged;
        movementController.OnJetpackActiveChanged -= HandleJetpackActiveChanged;
        movementController.OnGroundStateChanged -= HandleGroundStateChanged;
    }

    private void SyncFromMovement()
    {
        if (movementController == null)
            return;

        isGrounded = movementController.IsGrounded();
        isSprinting = movementController.IsSprinting();
        isJetpackActive = movementController.IsJetpackActive() && movementController.IsJetpackConsumingFuel();
        currentGroundTag = movementController.GetGroundTag();
    }

    private void HandleSprintChanged(bool sprinting)
    {
        isSprinting = sprinting;
    }

    private void HandleJetpackActiveChanged(bool active)
    {
        SyncFromMovement();
        RefreshJetpackAudio();
    }

    private void HandleGroundStateChanged(bool grounded, string groundTag)
    {
        isGrounded = grounded;
        currentGroundTag = groundTag;
    }

    public void PlayFootstep()
    {
        SyncFromMovement();

        if (isJetpackActive)
            return;

        AudioCueKey footstepCue = GetWalkCue(currentGroundTag);
        AudioService?.ChangePitch(footstepCue, isSprinting ? sprintPitch : walkPitch);
        AudioService?.PlayOneShot(footstepCue);
    }

    public void PlayLanding()
    {
        SyncFromMovement();
        PlayLanding(currentGroundTag);
    }

    private void PlayLanding(string groundTag)
    {
        AudioService?.PlayOneShot(GetJumpCue(groundTag));
    }

    private void RefreshJetpackAudio()
    {
        if (isJetpackActive)
        {
            if (!isJetpackAudioPlaying)
            {
                AudioService?.Play(AudioCueKey.PlayerJetpack);
                isJetpackAudioPlaying = true;
            }

            return;
        }

        StopJetpackAudio();
    }

    private void StopJetpackAudio()
    {
        if (!isJetpackAudioPlaying)
            return;

        AudioService?.Stop(AudioCueKey.PlayerJetpack);
        isJetpackAudioPlaying = false;
    }

    private AudioCueKey GetWalkCue(string groundTag)
    {
        if (ContainsSurface(groundTag, "Metal"))
            return AudioCueKey.PlayerMetalWalk;

        if (ContainsSurface(groundTag, "Grass"))
            return AudioCueKey.PlayerWalk;

        return AudioCueKey.PlayerWalkBase;
    }

    private AudioCueKey GetJumpCue(string groundTag)
    {
        if (ContainsSurface(groundTag, "Metal"))
            return AudioCueKey.PlayerMetalJump;

        if (ContainsSurface(groundTag, "Grass"))
            return AudioCueKey.PlayerJump;

        return AudioCueKey.PlayerJumpBase;
    }

    private bool ContainsSurface(string groundTag, string surface)
    {
        return !string.IsNullOrEmpty(groundTag)
            && groundTag.IndexOf(surface, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private IAudioService AudioService
    {
        get
        {
            if (audioService == null)
                ResolveAudioService();

            return audioService;
        }
    }

    private void ResolveReferences()
    {
        if (movementController == null)
            Debug.LogWarning("[PlayerAudioFeedback] Asigna MovementController por Inspector.", this);

        ResolveAudioService();
    }

    private void ResolveAudioService()
    {
        audioService = audioServiceBehaviour as IAudioService;

        if (audioService == null && audioServiceBehaviour != null)
            Debug.LogWarning("[PlayerAudioFeedback] El Audio Service asignado no implementa IAudioService.", this);

        if (audioService == null && !hasLoggedMissingAudioService)
        {
            hasLoggedMissingAudioService = true;
            Debug.LogWarning("[PlayerAudioFeedback] Asigna un AudioService u otro IAudioService por Inspector.", this);
        }
    }
}
