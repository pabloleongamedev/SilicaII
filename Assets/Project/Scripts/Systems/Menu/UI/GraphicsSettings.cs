/*
 * Arquitectura: Menu/UI
 * Script: GraphicsSettings
 * Rol: Presenter/controlador de configuracion visual. Lee y escribe por interfaces, no por PlayerPrefs directo.
 * Relaciones: Consume GameSettingsService o cualquier IGameSettingsReader/Writer asignado por Inspector.
 * Riesgo arquitectonico mitigado: si la persistencia cambia, esta UI no conoce PlayerPrefs ni singletons.
 */
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Image brightnessOverlay;

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
        RenderFromSettings();
        Subscribe();
    }

    private void OnDestroy()
    {
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.RemoveListener(ApplyBrightness);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(ApplyFullscreen);
    }

    private void RenderFromSettings()
    {
        if (settingsReader == null)
            return;

        if (brightnessSlider != null)
            brightnessSlider.value = settingsReader.Brightness;

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = settingsReader.Fullscreen;

        ApplyBrightnessVisual(settingsReader.Brightness);
        Screen.fullScreen = settingsReader.Fullscreen;
    }

    private void Subscribe()
    {
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.AddListener(ApplyBrightness);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(ApplyFullscreen);
    }

    private void ApplyBrightness(float value)
    {
        ApplyBrightnessVisual(value);

        if (settingsWriter == null)
            return;

        settingsWriter.Brightness = value;
        settingsWriter.Save();
    }

    private void ApplyBrightnessVisual(float value)
    {
        if (brightnessOverlay == null)
            return;

        Color color = brightnessOverlay.color;
        color.a = 1f - Mathf.Clamp01(value);
        brightnessOverlay.color = color;
    }

    private void ApplyFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        if (settingsWriter == null)
            return;

        settingsWriter.Fullscreen = isFullscreen;
        settingsWriter.Save();
    }

    private void ResolveSettingsService()
    {
        settingsReader = settingsServiceBehaviour as IGameSettingsReader;
        settingsWriter = settingsServiceBehaviour as IGameSettingsWriter;

        if (settingsReader == null || settingsWriter == null)
            Debug.LogWarning("[GraphicsSettings] Asigna un GameSettingsService que implemente IGameSettingsReader/IGameSettingsWriter.", this);
    }
}
