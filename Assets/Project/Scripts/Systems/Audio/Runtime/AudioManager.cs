/*
 * Arquitectura: Audio/Runtime
 * Script: AudioManager
 * Rol: Facade runtime global de audio basado en singleton y nombres de sonido.
 * Modulo: Gestiona reproduccion de audio general, UI y sonidos del jugador.
 * Relaciones: Implementa IAudioService para PlayerAudio, ScannerTrigger u otros consumidores asignables por Inspector.
 * Riesgo arquitectonico: mantiene singleton interno de compatibilidad y strings no verificables por compilador; debe migrar a AudioCue_SO.
 * Uso como referencia: conservar como adapter legacy mientras consumidores migran a un servicio de audio explicito.
 */
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour, IAudioService
{
    public static AudioManager Instance;
    [Header("Sounds List")]
    public Sound[] sounds;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(transform.root.gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = 0.0f;
        }
    }
        public void Play(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Play();
        }
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Stop();
        }
    }
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
        [HideInInspector]
        public AudioSource source;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangePitch(string name, float pitch)
    {
        // Buscamos el sonido en la lista
        Sound s = System.Array.Find(sounds, sound => sound.name == name);

        if (s != null)
        {
            // Cambiamos el pitch del AudioSource que se creó en Awake
            s.source.pitch = pitch;
        }
        else
        {
            Debug.LogWarning("AudioManager: No se pudo cambiar el pitch. Sonido no encontrado: " + name);
        }
    }
}
