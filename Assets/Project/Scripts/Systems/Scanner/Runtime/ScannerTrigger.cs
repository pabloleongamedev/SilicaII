/*
 * Arquitectura: Scanner/Runtime
 * Script: ScannerTrigger
 * Rol: Feedback visual/audio del scanner del Player.
 * Modulo: Escucha ScannerEvents y ejecuta animacion/audio cuando un IInteractable escaneable confirma que puede escanearse.
 * Relaciones: ScannableObject no conoce este componente; solo publica ScannerEvents.RequestScanFeedback.
 * Uso como referencia: ScannerTrigger ya no lee input ni decide que objeto se escanea; Interaction es la entrada oficial.
 */
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class ScannerTrigger : MonoBehaviour
{
    [Header("Audio Service")]
    [FormerlySerializedAs("audioManager")]
    [SerializeField] private MonoBehaviour audioServiceBehaviour;
    [SerializeField] private AudioCue_SO scannerCue;

    [Header("Scanner Visual")]
    [FormerlySerializedAs("PivotScan")]
    [SerializeField] private GameObject pivotScan;
    [FormerlySerializedAs("audioScanner")]
    [SerializeField] private AudioSource scannerAudioSource;
    [SerializeField] private string startScanTrigger = "StartScan";
    [SerializeField] private float visualDuration = 0.6f;

    [Header("Texture Effect")]
    [SerializeField] private float textureScrollSpeed = 2f;

    private Animator animator;
    private Renderer meshRenderer;
    private IAudioService audioService;
    private Coroutine stopRoutine;
    private bool hasLoggedMissingAudioService;

    private void Awake()
    {
        ResolveAudioService();
        ResolveVisualReferences();
    }

    private void OnEnable()
    {
        ResolveAudioService();
        ScannerEvents.OnScanFeedbackRequested += HandleScanFeedbackRequested;
    }

    private void OnDisable()
    {
        ScannerEvents.OnScanFeedbackRequested -= HandleScanFeedbackRequested;
        StopScanning();
    }

    private void Update()
    {
        if (pivotScan != null && pivotScan.activeSelf && meshRenderer != null)
        {
            float offset = Time.time * textureScrollSpeed;
            meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }

    private void HandleScanFeedbackRequested(IScannable scannable)
    {
        StartScanning();
    }

    private void StartScanning()
    {
        ResolveVisualReferences();

        if (pivotScan == null)
            return;

        pivotScan.SetActive(true);
        if (scannerCue != null)
            AudioService?.Play(scannerCue);
        else
            AudioService?.Play("Scannersound");

        if (animator != null && !string.IsNullOrEmpty(startScanTrigger))
            animator.SetTrigger(startScanTrigger);

        if (stopRoutine != null)
            StopCoroutine(stopRoutine);

        if (visualDuration > 0f)
            stopRoutine = StartCoroutine(StopAfterDelay());
    }

    private void StopScanning()
    {
        if (stopRoutine != null)
        {
            StopCoroutine(stopRoutine);
            stopRoutine = null;
        }

        if (pivotScan != null)
            pivotScan.SetActive(false);

        if (scannerCue != null)
            AudioService?.Stop(scannerCue);
        else
            AudioService?.Stop("Scannersound");

        if (animator != null && !string.IsNullOrEmpty(startScanTrigger))
            animator.ResetTrigger(startScanTrigger);

        if (scannerAudioSource != null)
            scannerAudioSource.Stop();
    }

    private IEnumerator StopAfterDelay()
    {
        yield return new WaitForSeconds(visualDuration);
        stopRoutine = null;
        StopScanning();
    }

    private void ResolveVisualReferences()
    {
        if (pivotScan == null)
            return;

        if (animator == null)
            animator = pivotScan.GetComponent<Animator>();

        if (meshRenderer == null)
            meshRenderer = pivotScan.GetComponentInChildren<Renderer>(true);
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
            Debug.LogWarning("[ScannerTrigger] El Audio Service asignado no implementa IAudioService.", this);

        if (audioService == null && !hasLoggedMissingAudioService)
        {
            hasLoggedMissingAudioService = true;
            Debug.LogWarning("[ScannerTrigger] Asigna AudioService u otro IAudioService por Inspector.", this);
        }
    }

    private MonoBehaviour FindLocalAudioServiceBehaviour()
    {
        // ScannerTrigger vive en el Player. Esta busqueda se limita al arbol local del Player,
        // permitiendo resolver un servicio de audio hijo sin volver a depender de singletons globales.
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
