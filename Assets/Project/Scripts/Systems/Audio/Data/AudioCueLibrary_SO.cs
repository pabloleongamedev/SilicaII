/*
 * Arquitectura: Audio/Data
 * Script: AudioCueLibrary_SO
 * Rol: Fuente central de referencias AudioCue_SO para toda la escena.
 * Relaciones: AudioService consulta esta libreria; PlayerAudioFeedback, ScannerTrigger, UI y Notification solo piden AudioCueKey.
 * Uso como referencia: agrega nuevos sonidos aqui para evitar duplicar asignaciones por sistema.
 */
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Audio/Audio Cue Library")]
public class AudioCueLibrary_SO : ScriptableObject
{
    [Header("Player")]
    [SerializeField] private AudioCue_SO playerWalk;
    [SerializeField] private AudioCue_SO playerMetalWalk;
    [SerializeField] private AudioCue_SO playerJump;
    [SerializeField] private AudioCue_SO playerMetalJump;
    [SerializeField] private AudioCue_SO playerJetpack;

    [Header("Scanner")]
    [SerializeField] private AudioCue_SO scannerScan;

    [Header("Notifications")]
    [SerializeField] private AudioCue_SO notificationInfo;
    [SerializeField] private AudioCue_SO notificationSuccess;
    [SerializeField] private AudioCue_SO notificationWarning;
    [SerializeField] private AudioCue_SO notificationError;

    [Header("UI")]
    [SerializeField] private AudioCue_SO uiHover;
    [SerializeField] private AudioCue_SO uiClick;
    [SerializeField] private AudioCue_SO uiApply;
    [SerializeField] private AudioCue_SO uiReset;
    [SerializeField] private AudioCue_SO uiSliderTick;
    [SerializeField] private AudioCue_SO uiSliderLimit;
    [SerializeField] private AudioCue_SO uiError;

    [Header("Feedback Futuro")]
    [SerializeField] private AudioCue_SO feedbackHit;
    [SerializeField] private AudioCue_SO feedbackExplosion;
    [SerializeField] private AudioCue_SO feedbackPickup;

    [Header("Extensiones")]
    [SerializeField] private AudioCueEntry[] customCues;

    public AudioCue_SO Get(AudioCueKey key)
    {
        switch (key)
        {
            case AudioCueKey.PlayerWalk: return playerWalk;
            case AudioCueKey.PlayerMetalWalk: return playerMetalWalk;
            case AudioCueKey.PlayerJump: return playerJump;
            case AudioCueKey.PlayerMetalJump: return playerMetalJump;
            case AudioCueKey.PlayerJetpack: return playerJetpack;
            case AudioCueKey.ScannerScan: return scannerScan;
            case AudioCueKey.NotificationInfo: return notificationInfo;
            case AudioCueKey.NotificationSuccess: return notificationSuccess;
            case AudioCueKey.NotificationWarning: return notificationWarning;
            case AudioCueKey.NotificationError: return notificationError;
            case AudioCueKey.UIHover: return uiHover;
            case AudioCueKey.UIClick: return uiClick;
            case AudioCueKey.UIApply: return uiApply;
            case AudioCueKey.UIReset: return uiReset;
            case AudioCueKey.UISliderTick: return uiSliderTick;
            case AudioCueKey.UISliderLimit: return uiSliderLimit;
            case AudioCueKey.UIError: return uiError;
            case AudioCueKey.FeedbackHit: return feedbackHit;
            case AudioCueKey.FeedbackExplosion: return feedbackExplosion;
            case AudioCueKey.FeedbackPickup: return feedbackPickup;
        }

        return GetCustom(key);
    }

    private AudioCue_SO GetCustom(AudioCueKey key)
    {
        if (customCues == null)
            return null;

        foreach (var entry in customCues)
        {
            if (entry.Key == key)
                return entry.Cue;
        }

        return null;
    }

    [Serializable]
    private struct AudioCueEntry
    {
        [SerializeField] private AudioCueKey key;
        [SerializeField] private AudioCue_SO cue;

        public AudioCueKey Key => key;
        public AudioCue_SO Cue => cue;
    }
}
