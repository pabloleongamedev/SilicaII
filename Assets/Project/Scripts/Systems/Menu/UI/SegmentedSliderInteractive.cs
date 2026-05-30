/*
 * Arquitectura: Menu/UI
 * Script: SegmentedSliderInteractive
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Se conecta con Menu/Settings para iniciar, cargar y configurar partida.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.UI;

public class SegmentedSliderInteractive : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Button[] segmentButtons;
    [SerializeField] private Image[] segmentImages;
    [SerializeField] private MonoBehaviour audioServiceBehaviour;

    [Header("Colors")]
    [SerializeField] private Color activeColor = Color.cyan;
    [SerializeField] private Color inactiveColor = new Color(0.35f, 0.55f, 0.55f, 1f);

    private IAudioService audioService;

    private void Start()
    {
        ResolveAudioService();

        if (slider == null)
        {
            Debug.LogWarning("[SegmentedSliderInteractive] Asigna slider por Inspector.", this);
            return;
        }

        if (segmentButtons == null || segmentButtons.Length == 0)
        {
            Debug.LogWarning("[SegmentedSliderInteractive] Asigna segmentButtons por Inspector.", this);
            return;
        }

        slider.minValue = 1f;
        slider.maxValue = segmentButtons.Length;
        slider.wholeNumbers = true;

        for (int i = 0; i < segmentButtons.Length; i++)
        {
            int index = i;
            segmentButtons[i].onClick.AddListener(() => OnSegmentClicked(index));
        }

        slider.onValueChanged.AddListener(UpdateSegments);
        UpdateSegments(slider.value);
    }

    private void OnSegmentClicked(int index)
    {
        bool changed = !Mathf.Approximately(slider.value, index + 1);
        slider.value = index + 1;

        if (changed)
            audioService?.PlayOneShot(AudioCueKey.UISliderTick);
        else
            audioService?.PlayOneShot(AudioCueKey.UISliderLimit);
    }

    private void UpdateSegments(float value)
    {
        int activeCount = Mathf.RoundToInt(value);

        for (int i = 0; i < segmentImages.Length; i++)
        {
            segmentImages[i].color = i < activeCount ? activeColor : inactiveColor;
        }
    }

    private void ResolveAudioService()
    {
        audioService = audioServiceBehaviour as IAudioService;

        if (audioService == null && audioServiceBehaviour != null)
            Debug.LogWarning("[SegmentedSliderInteractive] El Audio Service asignado no implementa IAudioService.", this);
    }
}
