/*
 * Arquitectura: Audio/Runtime
 * Script: PlayerAudio
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona reproduccion de audio general, UI y sonidos del jugador.
 * Relaciones: Es usado por UI, Player y Scanner para reproducir clips por nombre o acciones.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAudio : MonoBehaviour
{
    [Header("Input Action")]
    public InputActionReference walkAction;
    public InputActionReference runAction;
    public InputActionReference jetPackAction;
    private bool isWalkingSoundPlaying = false;
    private bool isJetpackSoundPlaying = false;
    private string currentGroundTag = "Ground";
    private bool isGrounded = false; // This should be set based on your player's grounded state
    private void OnEnable()
    {
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
                AudioManager.Instance.Play(soundToPlay);
                isWalkingSoundPlaying=true;
            }
        float targetPitch = isRunning ? 1.5f : 1f; // Adjust pitch for running
        AudioManager.Instance.ChangePitch(soundToPlay, targetPitch);
        }
        else
        {
            if (isWalkingSoundPlaying)
            {
                AudioManager.Instance.Stop("Playerwalksound");
                AudioManager.Instance.Stop("PlayerwalkMetalsound");
                isWalkingSoundPlaying = false;
            }
        }
        if (isJetPackActive)
        {
            if (!isJetpackSoundPlaying)
            {
                AudioManager.Instance.Play("Playerjetpacksound");
                isJetpackSoundPlaying=true;
            }
        }else
        {
            if (isJetpackSoundPlaying)
            {
                AudioManager.Instance.Stop("Playerjetpacksound");
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
            AudioManager.Instance.Play(jumpSound);

            // 3. Actualizamos el estado
            isGrounded = true;
            currentGroundTag = collision.gameObject.tag;
        }
    }
    
}