/*
 * Arquitectura: Audio/Runtime
 * Script: AudioService
 * Rol: Servicio runtime de audio por escena/prefab. Resuelve AudioCueKey contra AudioCueLibrary_SO y reproduce AudioCue_SO.
 * Relaciones: PlayerAudioFeedback, ScannerTrigger, UIAudioManager y NotificationAudioController dependen de IAudioService.
 * Riesgo arquitectonico mitigado: evita singletons y permite reemplazar la implementacion por Inspector.
 * Uso como referencia: nuevos sistemas deben pedir AudioCueKey; las referencias AudioCue_SO viven en AudioCueLibrary_SO.
 */
using System.Collections.Generic;
using UnityEngine;

public class AudioService : MonoBehaviour, IAudioService
{
    [Header("Audio Library")]
    [SerializeField] private AudioCueLibrary_SO audioLibrary;
    
    private readonly Dictionary<string, AudioSource> sourcesByID = new();
    private readonly HashSet<AudioCueKey> missingCueWarnings = new();

    public void Play(AudioCueKey key)
    {
        var source = ResolveSource(key);
        source?.Play();
    } 

    public void PlayOneShot(AudioCueKey key)
    {
        var source = ResolveSource(key);

        if (source == null || source.clip == null)
            return;

        source.PlayOneShot(source.clip);
    }

    public void Stop(AudioCueKey key)
    {
        var source = ResolveSource(key);
        source?.Stop();
    }

    public void ChangePitch(AudioCueKey key, float pitch)
    {
        var source = ResolveSource(key);
        
        if (source != null)
            source.pitch = pitch;
    }

    private AudioSource ResolveSource(AudioCueKey key)
    {
        var cue = audioLibrary != null ? audioLibrary.Get(key) : null;

        if (cue == null || cue.Clip == null)
        {
            WarnMissingCue(key, cue);
            return null;
        }

        Register(cue);
        sourcesByID.TryGetValue(cue.CueID, out var source);
        return source;
    }

    private void WarnMissingCue(AudioCueKey key, AudioCue_SO cue)
    {
        if (missingCueWarnings.Contains(key))
            return;

        missingCueWarnings.Add(key);
        string reason = audioLibrary == null
            ? "AudioCueLibrary_SO no esta asignada"
            : cue == null
                ? "AudioCue_SO no esta asignado en la libreria"
                : "AudioCue_SO no tiene clip";
        Debug.LogWarning($"[AudioService] No se puede reproducir {key}: {reason}.", this);
    }

    private void Register(AudioCue_SO cue)
    {
        if (cue == null || cue.Clip == null || string.IsNullOrEmpty(cue.CueID) || sourcesByID.ContainsKey(cue.CueID))
            return;

        var source = gameObject.AddComponent<AudioSource>();
        source.clip = cue.Clip;
        source.volume = cue.Volume;
        source.pitch = cue.Pitch;
        source.loop = cue.Loop;
        source.spatialBlend = 0f;
        sourcesByID.Add(cue.CueID, source);
    }

    public bool HasCue(AudioCueKey key)
    {
        var cue = audioLibrary != null ? audioLibrary.Get(key) : null;
        return cue != null && !string.IsNullOrEmpty(cue.CueID);
    }
}
