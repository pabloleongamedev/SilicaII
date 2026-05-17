/*
 * Arquitectura: Menu/UI
 * Script: OptionsMenuManager
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Usa IGameSettingsReader/IGameSettingsWriter para no depender de GameSettings.Instance.
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

    [Header("Audio")]
    [SerializeField] private SoundSettings soundSettings;

    [Header("Settings Service")]
    [SerializeField] private MonoBehaviour settingsServiceBehaviour;

    private IGameSettingsReader settingsReader;
    private IGameSettingsWriter settingsWriter;

    private void Awake()
    {
        ResolveSettingsService();
    }

    private void Start()
    {
        ResolveSettingsService();
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
        if (settingsReader == null)
            return;

        brightnessSlider.value = settingsReader.Brightness;
        musicSlider.value = settingsReader.MusicVolume;
        effectsSlider.value = settingsReader.EffectsVolume;
        masterVolumeSlider.value = settingsReader.GetMasterVolumeSegments();
    }

    private void ApplySettings()
    {
        if (settingsWriter == null)
            return;

        settingsWriter.Brightness = brightnessSlider.value;
        settingsWriter.MusicVolume = musicSlider.value;
        settingsWriter.EffectsVolume = effectsSlider.value;
        settingsWriter.SetMasterVolumeFromSegments(masterVolumeSlider.value);
        settingsWriter.Save();

        if (soundSettings != null)
        {
            soundSettings.ApplySound();
        }
    }

    private void ResetSettings()
    {
        if (settingsWriter == null)
            return;

        settingsWriter.ResetToDefaults();
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

    private void ResolveSettingsService()
    {
        if (settingsServiceBehaviour == null)
            settingsServiceBehaviour = GetComponentInParent<GameSettingsService>();

        if (settingsServiceBehaviour == null)
            settingsServiceBehaviour = gameObject.AddComponent<GameSettingsService>();

        settingsReader = settingsServiceBehaviour as IGameSettingsReader;
        settingsWriter = settingsServiceBehaviour as IGameSettingsWriter;

        if (settingsReader == null || settingsWriter == null)
            Debug.LogWarning("[OptionsMenuManager] Asigna un GameSettingsService que implemente IGameSettingsReader/IGameSettingsWriter.", this);
    }
}
