/*
 * Arquitectura: Audio/Runtime
 * Script: PlayerAudioFeedback
 * Rol: Feedback audiovisual del Player. Observa MovementController y solicita AudioCueKey al IAudioService central.
 * Relaciones: No lee input ni colisiones propias; PlayerInputHandler alimenta MovementController y este publica estado real.
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
    private bool hasMoveInput;
    private bool isGrounded;
    private bool isSprinting;
    private bool isJetpackActive;
    private string currentGroundTag = "Ground";
    private AudioCueKey activeWalkCue = AudioCueKey.None;
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
        RefreshWalkAudio();
        RefreshJetpackAudio();
    }

    private void OnDisable()
    {
        Unsubscribe();
        StopWalkAudio();
        StopJetpackAudio();
    }

    private void Subscribe()
    {
        if (movementController == null)
            return;

        movementController.OnMoveInputChanged += HandleMoveInputChanged;
        movementController.OnSprintChanged += HandleSprintChanged;
        movementController.OnLanded += HandleLanded;
        movementController.OnJetpackActiveChanged += HandleJetpackActiveChanged;
        movementController.OnGroundStateChanged += HandleGroundStateChanged;
    }

    private void Unsubscribe()
    {
        if (movementController == null)
            return;

        movementController.OnMoveInputChanged -= HandleMoveInputChanged;
        movementController.OnSprintChanged -= HandleSprintChanged;
        movementController.OnLanded -= HandleLanded;
        movementController.OnJetpackActiveChanged -= HandleJetpackActiveChanged;
        movementController.OnGroundStateChanged -= HandleGroundStateChanged;
    }

    private void SyncFromMovement()
    {
        if (movementController == null)
            return;

        hasMoveInput = movementController.GetMoveInput().sqrMagnitude > 0.01f;
        isGrounded = movementController.IsGrounded();
        isSprinting = movementController.IsSprinting();
        isJetpackActive = movementController.IsJetpackActive();
        currentGroundTag = movementController.GetGroundTag();
    }

    private void HandleMoveInputChanged(Vector2 input)
    {
        hasMoveInput = input.sqrMagnitude > 0.01f;
        RefreshWalkAudio();
    }

    private void HandleSprintChanged(bool sprinting)
    {
        isSprinting = sprinting;
        RefreshWalkAudio();
    }

    private void HandleLanded(string groundTag)
    {
        AudioService?.Play(groundTag == "MetalGround" ? AudioCueKey.PlayerMetalJump : AudioCueKey.PlayerJump);
    }

    private void HandleJetpackActiveChanged(bool active)
    {
        isJetpackActive = active;
        RefreshJetpackAudio();
    }

    private void HandleGroundStateChanged(bool grounded, string groundTag)
    {
        bool changedSurface = currentGroundTag != groundTag;
        isGrounded = grounded;
        currentGroundTag = groundTag;

        if (changedSurface)
            StopWalkAudio();

        RefreshWalkAudio();
    }

    private void RefreshWalkAudio()
    {
        if (!hasMoveInput || !isGrounded)
        {
            StopWalkAudio();
            return;
        }

        AudioCueKey nextCue = IsMetalGround() ? AudioCueKey.PlayerMetalWalk : AudioCueKey.PlayerWalk;

        if (activeWalkCue != AudioCueKey.None && activeWalkCue != nextCue)
            StopWalkAudio();

        if (activeWalkCue == AudioCueKey.None)
        {
            activeWalkCue = nextCue;
            AudioService?.Play(activeWalkCue);
        }

        AudioService?.ChangePitch(activeWalkCue, isSprinting ? sprintPitch : walkPitch);
    }

    private void StopWalkAudio()
    {
        if (activeWalkCue == AudioCueKey.None)
            return;

        AudioService?.Stop(activeWalkCue);
        activeWalkCue = AudioCueKey.None;
    }

    private void RefreshJetpackAudio()
    {
        if (isJetpackActive)
        {
            AudioService?.Play(AudioCueKey.PlayerJetpack);
            return;
        }

        StopJetpackAudio();
    }

    private void StopJetpackAudio()
    {
        AudioService?.Stop(AudioCueKey.PlayerJetpack);
    }

    private bool IsMetalGround()
    {
        return currentGroundTag == "MetalGround";
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
