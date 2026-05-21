/*
 * Arquitectura: Scanner/Runtime
 * Script: ScannerTrigger
 * Rol: Feedback visual/audio del scanner del Player.
 * Modulo: Escucha ScannerFeedbackEventChannel_SO y ejecuta animacion/audio cuando un IInteractable escaneable confirma que puede escanearse.
 * Relaciones: ItemPickup en modo Scannable no conoce este componente; solo publica por EventChannel.
 * Uso como referencia: ScannerTrigger ya no lee input ni decide que objeto se escanea; Interaction es la entrada oficial.
 */
using System.Collections;
using UnityEngine;

public class ScannerTrigger : MonoBehaviour
{
    [Header("Audio Service")]
    [SerializeField] private MonoBehaviour audioServiceBehaviour;

    [Header("Events")]
    [SerializeField] private ScannerFeedbackEventChannel_SO scannerFeedbackChannel;

    [Header("Scanner Visual")]
    [SerializeField] private GameObject pivotScan;
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
        if (scannerFeedbackChannel != null)
            scannerFeedbackChannel.Raised += HandleScanFeedbackRequested;
    }

    private void OnDisable()
    {
        if (scannerFeedbackChannel != null)
            scannerFeedbackChannel.Raised -= HandleScanFeedbackRequested;
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
        AudioService?.Play(AudioCueKey.ScannerScan);

        if (CanUseAnimatorTrigger())
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

        AudioService?.Stop(AudioCueKey.ScannerScan);

        if (CanUseAnimatorTrigger())
            animator.ResetTrigger(startScanTrigger);
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

    private bool CanUseAnimatorTrigger()
    {
        return animator != null
            && animator.runtimeAnimatorController != null
            && !string.IsNullOrEmpty(startScanTrigger);
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
            Debug.LogWarning("[ScannerTrigger] El Audio Service asignado no implementa IAudioService.", this);

        if (audioService == null && !hasLoggedMissingAudioService)
        {
            hasLoggedMissingAudioService = true;
            Debug.LogWarning("[ScannerTrigger] Asigna AudioService u otro IAudioService por Inspector.", this);
        }
    }
}
