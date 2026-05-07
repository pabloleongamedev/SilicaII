using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    [Header("UI References")]
    public Slider brightnessSlider;
    public Toggle fullscreenToggle;
    public Image brightnessOverlay;

    void Start()
    {
        // Cargar valores guardados
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 0.8f);

        ApplyBrightness(brightnessSlider.value);
        ApplyFullscreen(fullscreenToggle.isOn);

        // Conectar eventos
        brightnessSlider.onValueChanged.AddListener(ApplyBrightness);
        fullscreenToggle.onValueChanged.AddListener(ApplyFullscreen);
    }

    void ApplyBrightness(float value)
    {
        if (brightnessOverlay != null)
        {
            Color c = brightnessOverlay.color;
            c.a = 1f - value; // más brillo = menos opacidad
            brightnessOverlay.color = c;
        }

        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save();
    }

    void ApplyFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
