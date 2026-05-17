/*
 * Arquitectura: Audio/Runtime
 * Script: PlayerAudio
 * Rol: Adapter runtime de audio del jugador. Traduce input/collision a intenciones de sonido.
 * Modulo: Gestiona reproduccion de audio general, UI y sonidos del jugador.
 * Relaciones: Lee InputActionReference y tags de suelo; usa IAudioService asignado por Inspector.
 * Riesgo arquitectonico: aun usa strings globales; debe migrar a AudioCue_SO para eliminar typos de runtime.
 * Uso como referencia: separa audio de PlayerInputHandler y abre una frontera hacia servicio de audio explicito.
 */
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Service")]
    [SerializeField] private MonoBehaviour audioServiceBehaviour;

    [Header("Input Action")]
    public InputActionReference walkAction;
    public InputActionReference runAction;
    public InputActionReference jetPackAction;
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
        float inputMovement = walkAction.action.ReadValue<Vector2>().magnitude;
        bool isRunning = runAction.action.IsPressed();
        bool isJetPackActive = jetPackAction.action.IsPressed();
        if (inputMovement > 0.1f && isGrounded)
        {
            string soundToPlay = (currentGroundTag == "MetalGround") ? "PlayerwalkMetalsound" : "Playerwalksound";
            if (!isWalkingSoundPlaying)
            {
                AudioService?.Play(soundToPlay);
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
                AudioService?.Play("Playerjetpacksound");
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
            string jumpSound = (collision.gameObject.CompareTag("MetalGround"))
                               ? "PlayerJumpMetalsound"
                               : "Playerjumpsound";

            // 2. Reproducimos el sonido detectado
            AudioService?.Play(jumpSound);

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
        audioService = audioServiceBehaviour as IAudioService;

        if (audioService == null && audioServiceBehaviour != null)
            Debug.LogWarning("[PlayerAudio] El Audio Service asignado no implementa IAudioService.", this);

        if (audioService == null && !hasLoggedMissingAudioService)
        {
            hasLoggedMissingAudioService = true;
            Debug.LogWarning("[PlayerAudio] Asigna un AudioManager u otro IAudioService por Inspector.", this);
        }
    }
    
}
