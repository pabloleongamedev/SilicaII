/*
 * Arquitectura: Menu/UI
 * Script: OptionsMenuManager
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Usa IGameSettingsReader/IGameSettingsWriter para no depender de una configuracion global.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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

    [Header("Toggles")]
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Audio")]
    [SerializeField] private SoundSettings soundSettings;

    [Header("Graphics")]
    [SerializeField] private GraphicsSettings graphicsSettings;

    [Header("Settings Service")]
    [SerializeField] private MonoBehaviour settingsServiceBehaviour;

    private IGameSettingsReader settingsReader;
    private IGameSettingsWriter settingsWriter;
    private bool warnedMissingFullscreenToggle;

    private void Awake()
    {
        ResolveSettingsService();
    }

    private void Start()
    {
        ResolveSettingsService();
        ConfigureMasterSlider();
        LoadSettingsIntoUI(true);

        AddButtonListener(applyButton, ApplySettings, nameof(applyButton));
        AddButtonListener(resetButton, ResetSettings, nameof(resetButton));
        AddButtonListener(closeButton, CloseOptionsPanel, nameof(closeButton));
    }

    private void ConfigureMasterSlider()
    {
        if (masterVolumeSlider == null)
        {
            Debug.LogWarning("[OptionsMenuManager] Asigna Master Volume Slider por Inspector.", this);
            return;
        }

        masterVolumeSlider.minValue = 1f;
        masterVolumeSlider.maxValue = 10f;
        masterVolumeSlider.wholeNumbers = true;
    }

    private void LoadSettingsIntoUI(bool warnMissingFullscreenToggle = false)
    {
        if (settingsReader == null)
            return;

        SetSliderValue(brightnessSlider, settingsReader.Brightness, nameof(brightnessSlider));
        SetSliderValue(musicSlider, settingsReader.MusicVolume, nameof(musicSlider));
        SetSliderValue(effectsSlider, settingsReader.EffectsVolume, nameof(effectsSlider));
        SetSliderValue(masterVolumeSlider, settingsReader.GetMasterVolumeSegments(), nameof(masterVolumeSlider));

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = settingsReader.Fullscreen;
        }
        else if (warnMissingFullscreenToggle && !warnedMissingFullscreenToggle)
        {
            warnedMissingFullscreenToggle = true;
            Debug.LogWarning("[OptionsMenuManager] Asigna fullscreenToggle por Inspector.", this);
        }
    }

    private void ApplySettings()
    {
        if (settingsWriter == null)
            return;

        if (!HasRequiredSliders())
            return;

        settingsWriter.Brightness = brightnessSlider.value;
        settingsWriter.MusicVolume = musicSlider.value;
        settingsWriter.EffectsVolume = effectsSlider.value;
        settingsWriter.SetMasterVolumeFromSegments(masterVolumeSlider.value);

        if (fullscreenToggle != null)
            settingsWriter.Fullscreen = fullscreenToggle.isOn;

        settingsWriter.Save();

        if (soundSettings != null)
            soundSettings.ApplySound();

        if (graphicsSettings != null)
            graphicsSettings.ApplyGraphics();
    }

    private void ResetSettings()
    {
        if (settingsWriter == null)
            return;

        settingsWriter.ResetToDefaults();
        LoadSettingsIntoUI();

        if (soundSettings != null)
            soundSettings.ApplySound();

        if (graphicsSettings != null)
            graphicsSettings.ApplyGraphics();
    }

    private void CloseOptionsPanel()
    {
        if (optionsPanel == null)
        {
            Debug.LogWarning("[OptionsMenuManager] Asigna Options Panel por Inspector para cerrar opciones.", this);
            return;
        }

        optionsPanel.SetActive(false);
    }

    private void AddButtonListener(Button button, UnityEngine.Events.UnityAction action, string fieldName)
    {
        if (button == null)
        {
            Debug.LogWarning($"[OptionsMenuManager] Asigna {fieldName} por Inspector.", this);
            return;
        }

        button.onClick.AddListener(action);
    }

    private void SetSliderValue(Slider slider, float value, string fieldName)
    {
        if (slider == null)
        {
            Debug.LogWarning($"[OptionsMenuManager] Asigna {fieldName} por Inspector.", this);
            return;
        }

        slider.value = value;
    }

    private bool HasRequiredSliders()
    {
        if (brightnessSlider != null && musicSlider != null && effectsSlider != null && masterVolumeSlider != null)
            return true;

        Debug.LogWarning("[OptionsMenuManager] Faltan sliders de opciones por Inspector; no se aplican cambios.", this);
        return false;
    }

    private void ResolveSettingsService()
    {
        settingsReader = settingsServiceBehaviour as IGameSettingsReader;
        settingsWriter = settingsServiceBehaviour as IGameSettingsWriter;

        if (settingsReader == null || settingsWriter == null)
            Debug.LogWarning("[OptionsMenuManager] Asigna un GameSettingsService que implemente IGameSettingsReader/IGameSettingsWriter.", this);
    }
}
