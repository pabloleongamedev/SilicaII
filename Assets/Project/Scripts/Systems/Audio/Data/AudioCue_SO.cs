/*
 * Arquitectura: Audio/Data
 * Script: AudioCue_SO
 * Rol: Asset de authoring para identificar un sonido sin depender de strings sueltos en gameplay.
 * Relaciones: AudioCueLibrary_SO centraliza AudioCue_SO; AudioService los reproduce por AudioCueKey.
 * Riesgo arquitectonico mitigado: evita typos runtime como "Playerjetpacksound" repartidos entre sistemas.
 */
using UnityEngine;

[CreateAssetMenu(menuName = "SilicaII/Audio/Audio Cue")]
public class AudioCue_SO : ScriptableObject
{
    [SerializeField] private string cueID;
    [SerializeField] private AudioClip clip;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField, Range(0.1f, 3f)] private float pitch = 1f;
    [SerializeField] private bool loop;

    public string CueID => string.IsNullOrEmpty(cueID) ? name : cueID;
    public AudioClip Clip => clip;
    public float Volume => volume;
    public float Pitch => pitch;
    public bool Loop => loop;
}
