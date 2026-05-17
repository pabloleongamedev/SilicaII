/*
 * Arquitectura: Audio/Runtime
 * Script: AudioService
 * Rol: Servicio runtime de audio por escena/prefab. Registra AudioCue_SO y mantiene compatibilidad temporal con ids string.
 * Relaciones: PlayerAudio, ScannerTrigger y NotificationAudioController dependen de IAudioService, no de un singleton global.
 * Riesgo arquitectonico mitigado: elimina AudioManager.Instance y permite reemplazar la implementacion por Inspector.
 * Uso como referencia: nuevos sistemas deben usar AudioCue_SO; Play(string) queda como puente de migracion.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioService : MonoBehaviour, IAudioService
{
    [Header("Audio Cues")]
    [SerializeField] private AudioCue_SO[] cues;

    [Header("Legacy Cues")]
    [FormerlySerializedAs("sounds")]
    [SerializeField] private LegacyAudioCue[] legacyCues;

    private readonly Dictionary<string, AudioSource> sourcesByID = new Dictionary<string, AudioSource>();

    protected virtual void Awake()
    {
        BuildSources();
    }

    public void Play(AudioCue_SO cue)
    {
        if (cue != null)
            Play(cue.CueID);
    }

    public void Stop(AudioCue_SO cue)
    {
        if (cue != null)
            Stop(cue.CueID);
    }

    public void Play(string cueID)
    {
        if (!string.IsNullOrEmpty(cueID) && sourcesByID.TryGetValue(cueID, out var source) && source != null)
            source.Play();
    }

    public void Stop(string cueID)
    {
        if (!string.IsNullOrEmpty(cueID) && sourcesByID.TryGetValue(cueID, out var source) && source != null)
            source.Stop();
    }

    public void ChangePitch(string cueID, float pitch)
    {
        if (!string.IsNullOrEmpty(cueID) && sourcesByID.TryGetValue(cueID, out var source) && source != null)
            source.pitch = pitch;
    }

    private void BuildSources()
    {
        sourcesByID.Clear();

        if (cues != null)
        {
            foreach (var cue in cues)
            {
                if (cue != null && cue.Clip != null)
                    Register(cue.CueID, cue.Clip, cue.Volume, cue.Pitch, cue.Loop);
            }
        }

        if (legacyCues != null)
        {
            foreach (var cue in legacyCues)
            {
                if (cue != null && !string.IsNullOrEmpty(cue.name) && cue.clip != null)
                    Register(cue.name, cue.clip, cue.volume, cue.pitch, cue.loop);
            }
        }
    }

    private void Register(string cueID, AudioClip clip, float volume, float pitch, bool loop)
    {
        if (sourcesByID.ContainsKey(cueID))
            return;

        var source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.spatialBlend = 0f;
        sourcesByID.Add(cueID, source);
    }

    #pragma warning disable 0649
    [Serializable]
    private class LegacyAudioCue
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop;
        [HideInInspector] public AudioSource source;
    }
    #pragma warning restore 0649
}
