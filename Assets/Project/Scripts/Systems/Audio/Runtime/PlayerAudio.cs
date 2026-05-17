/*
 * Arquitectura: Audio/Runtime
 * Script: PlayerAudio
 * Rol: Adapter runtime de audio del jugador. Traduce input/collision a intenciones de sonido.
 * Modulo: Gestiona reproduccion de audio general, UI y sonidos del jugador.
 * Relaciones: Lee InputActionReference y tags de suelo; usa IAudioService asignado por Inspector.
 * Riesgo arquitectonico mitigado: nuevos sonidos se asignan como AudioCue_SO; los ids string quedan como fallback temporal.
 * Uso como referencia: separa audio de PlayerInputHandler y abre una frontera hacia servicio de audio explicito.
 */
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Service")]
    [FormerlySerializedAs("audioManager")]
    [SerializeField] private MonoBehaviour audioServiceBehaviour;

    [Header("Input Action")]
    [SerializeField] private InputActionReference walkAction;
    [SerializeField] private InputActionReference runAction;
    [SerializeField] private InputActionReference jetPackAction;

    [Header("Audio Cues")]
    [SerializeField] private AudioCue_SO walkCue;
    [SerializeField] private AudioCue_SO metalWalkCue;
    [SerializeField] private AudioCue_SO jumpCue;
    [SerializeField] private AudioCue_SO metalJumpCue;
    [SerializeField] private AudioCue_SO jetpackCue;
    private bool isWalkingSoundPlaying = false;
    private bool isJetpackSoundPlaying = false;
    private string currentGroundTag = "Ground";
    private bool isGrounded = false; // This should be set based on your player's grounded state
    private IAudioService audioService;
    private bool hasLoggedMissingAudioService;

    private void Awake()
    {
        ResolveAudioService();
    }

    private void OnEnable()
    {
        ResolveAudioService();
        if (walkAction != null)
            walkAction.action.Enable();
        if (runAction != null)
        {
            runAction.action.Enable();
        }
        if (jetPackAction != null)
        {
            jetPackAction.action.Enable();
        }
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputMovement = walkAction != null ? walkAction.action.ReadValue<Vector2>().magnitude : 0f;
        bool isRunning = runAction != null && runAction.action.IsPressed();
        bool isJetPackActive = jetPackAction != null && jetPackAction.action.IsPressed();
        if (inputMovement > 0.1f && isGrounded)
        {
            string soundToPlay = (currentGroundTag == "MetalGround") ? "PlayerwalkMetalsound" : "Playerwalksound";
            AudioCue_SO cueToPlay = currentGroundTag == "MetalGround" ? metalWalkCue : walkCue;
            if (!isWalkingSoundPlaying)
            {
                PlayCue(cueToPlay, soundToPlay);
                isWalkingSoundPlaying=true;
            }
        float targetPitch = isRunning ? 1.5f : 1f; // Adjust pitch for running
        AudioService?.ChangePitch(soundToPlay, targetPitch);
        }
        else
        {
            if (isWalkingSoundPlaying)
            {
                AudioService?.Stop("Playerwalksound");
                AudioService?.Stop("PlayerwalkMetalsound");
                isWalkingSoundPlaying = false;
            }
        }
        if (isJetPackActive)
        {
            if (!isJetpackSoundPlaying)
            {
                PlayCue(jetpackCue, "Playerjetpacksound");
                isJetpackSoundPlaying=true;
            }
        }else
        {
            if (isJetpackSoundPlaying)
            {
                AudioService?.Stop("Playerjetpacksound");
                isJetpackSoundPlaying = false;
            }
        }
    }
    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MetalGround"))
        {
            isGrounded = true;
            currentGroundTag = collision.gameObject.tag;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MetalGround"))
        {
            isGrounded = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MetalGround"))
        {
            // 1. Elegimos el sonido de salto/aterrizaje según la etiqueta
            bool isMetal = collision.gameObject.CompareTag("MetalGround");
            string jumpSound = isMetal
                               ? "PlayerJumpMetalsound"
                               : "Playerjumpsound";
            AudioCue_SO jumpCueToPlay = isMetal ? metalJumpCue : jumpCue;

            // 2. Reproducimos el sonido detectado
            PlayCue(jumpCueToPlay, jumpSound);

            // 3. Actualizamos el estado
            isGrounded = true;
            currentGroundTag = collision.gameObject.tag;
        }
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

    private void ResolveAudioService()
    {
        if (audioServiceBehaviour == null)
            audioServiceBehaviour = FindLocalAudioServiceBehaviour();

        audioService = audioServiceBehaviour as IAudioService;

        if (audioService == null && audioServiceBehaviour != null)
            Debug.LogWarning("[PlayerAudio] El Audio Service asignado no implementa IAudioService.", this);

        if (audioService == null && !hasLoggedMissingAudioService)
        {
            hasLoggedMissingAudioService = true;
            Debug.LogWarning("[PlayerAudio] Asigna un AudioService u otro IAudioService por Inspector.", this);
        }
    }

    private void PlayCue(AudioCue_SO cue, string fallbackID)
    {
        if (cue != null)
        {
            AudioService?.Play(cue);
            return;
        }

        AudioService?.Play(fallbackID);
    }

    private MonoBehaviour FindLocalAudioServiceBehaviour()
    {
        var localBehaviours = GetComponentsInChildren<MonoBehaviour>(true);

        foreach (var behaviour in localBehaviours)
        {
            if (behaviour != null && behaviour is IAudioService)
                return behaviour;
        }

        var parentBehaviours = GetComponentsInParent<MonoBehaviour>(true);

        foreach (var behaviour in parentBehaviours)
        {
            if (behaviour != null && behaviour is IAudioService)
                return behaviour;
        }

        return null;
    }
    
}
