using UnityEngine;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Exposed Parameter Names")]
    [SerializeField] private string masterVolumeParameter = "MasterVol";
    [SerializeField] private string musicVolumeParameter = "MusicVol";
    [SerializeField] private string sfxVolumeParameter = "SFXVol";

    private void Start()
    {
        ApplySound();
    }

    public void ApplySound()
    {
        DebugSetMixerVolume(masterVolumeParameter, GameSettings.Instance.MasterVolume);
        DebugSetMixerVolume(musicVolumeParameter, GameSettings.Instance.MusicVolume);
        DebugSetMixerVolume(sfxVolumeParameter, GameSettings.Instance.EffectsVolume);
    }

    public void PreviewMusicVolume(float value)
    {
        DebugSetMixerVolume(musicVolumeParameter, value);
    }

    public void PreviewSFXVolume(float value)
    {
        DebugSetMixerVolume(sfxVolumeParameter, value);
    }

    public void PreviewMasterVolume(float segments)
{
    segments = Mathf.Clamp(segments, 1f, 10f);

    float normalizedVolume = segments / 10f;
    GameSettings.Instance.MasterVolume = normalizedVolume;

    DebugSetMixerVolume(masterVolumeParameter, normalizedVolume);
}


    private void DebugSetMixerVolume(string parameterName, float normalizedVolume)
    {
        normalizedVolume = Mathf.Clamp(normalizedVolume, 0.5f, 1f);
        float volumeInDb = Mathf.Log10(normalizedVolume) * 20f;

        bool setOk = audioMixer.SetFloat(parameterName, volumeInDb);

        float currentValue;
        bool getOk = audioMixer.GetFloat(parameterName, out currentValue);

        Debug.Log(
            $"[AUDIO DEBUG] Param: {parameterName} | Normalized: {normalizedVolume} | dB: {volumeInDb} | SetOK: {setOk} | GetOK: {getOk} | Current: {currentValue}"
        );
    }
    

}