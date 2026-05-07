using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    [Header("Panel de Opciones")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Botones")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button closeButton;

    [Header("Sliders")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;
    [SerializeField] private Slider masterVolumeSlider;

    [Header("Audio")]
    [SerializeField] private SoundSettings soundSettings;

    private void Start()
    {
        ConfigureMasterSlider();
        LoadSettingsIntoUI();

        applyButton.onClick.AddListener(ApplySettings);
        resetButton.onClick.AddListener(ResetSettings);
        closeButton.onClick.AddListener(CloseOptionsPanel);
    }

    private void ConfigureMasterSlider()
    {
        masterVolumeSlider.minValue = 1f;
        masterVolumeSlider.maxValue = 10f;
        masterVolumeSlider.wholeNumbers = true;
    }

    private void LoadSettingsIntoUI()
    {
        brightnessSlider.value = GameSettings.Instance.Brightness;
        musicSlider.value = GameSettings.Instance.MusicVolume;
        effectsSlider.value = GameSettings.Instance.EffectsVolume;
        masterVolumeSlider.value = GameSettings.Instance.GetMasterVolumeSegments();
    }

    private void ApplySettings()
    {
        GameSettings.Instance.Brightness = brightnessSlider.value;
        GameSettings.Instance.MusicVolume = musicSlider.value;
        GameSettings.Instance.EffectsVolume = effectsSlider.value;
        GameSettings.Instance.SetMasterVolumeFromSegments(masterVolumeSlider.value);

        GameSettings.Instance.Save();

        if (soundSettings != null)
        {
            soundSettings.ApplySound();
        }
    }

    private void ResetSettings()
    {
        GameSettings.Instance.ResetToDefaults();
        LoadSettingsIntoUI();

        if (soundSettings != null)
        {
            soundSettings.ApplySound();
        }
    }

    private void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
    }
}