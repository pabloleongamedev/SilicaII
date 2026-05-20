/*
 * Arquitectura: Audio/Runtime
 * Script: UIAudioManager
 * Rol: Adaptador de audio UI por escena (sin singleton global).
 * Modulo: Reproduce sonidos UI mediante IAudioService.
 * Relaciones: Consume IAudioService; las referencias AudioCue_SO viven en AudioCueLibrary_SO.
 * Uso como referencia: UI audio queda en la escena activa y evita dependencia a Instance/DontDestroyOnLoad.
 */
using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    [Header("Audio Service")]
    [SerializeField] private MonoBehaviour audioServiceBehaviour;

    private IAudioService audioService;

    private void Awake()
    {
        ResolveAudioService();
    }

    public void PlayHover() => PlayUI(AudioCueKey.UIHover);
    public void PlayClick() => PlayUI(AudioCueKey.UIClick);
    public void PlayApply() => PlayUI(AudioCueKey.UIApply);
    public void PlayReset() => PlayUI(AudioCueKey.UIReset);
    public void PlaySliderTick() => PlayUI(AudioCueKey.UISliderTick);
    public void PlaySliderLimit() => PlayUI(AudioCueKey.UISliderLimit);
    public void PlayError() => PlayUI(AudioCueKey.UIError);

    private void PlayUI(AudioCueKey key) => audioService?.Play(key);

    private void ResolveAudioService()
    {
        if (audioServiceBehaviour == null)
            audioServiceBehaviour = GetComponentInParent<AudioService>();

        audioService = audioServiceBehaviour as IAudioService;

        if (audioService == null && audioServiceBehaviour != null)
            Debug.LogWarning("[UIAudioManager] El Audio Service asignado no implementa IAudioService.", this);
    }

}
