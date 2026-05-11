/*
 * Arquitectura: HUD/UI
 * Script: HUDManager
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona indicadores visuales persistentes de estado, tiempo, vitalidad y propulsor.
 * Relaciones: Presenta datos de runtime sin ser la fuente de reglas de gameplay.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] HealthBarSegments vitalityBar;
    [SerializeField] MissionTimer missionTimer;
    
    [Header("Textos de Estado")]
    [SerializeField] TMP_Text statusText;
    [SerializeField] TMP_Text diagnosticText;
    [SerializeField] TMP_Text vibraText;
    Color redColor = new Color(1f, 180f/255f, 171f/255f, 1f);

    void OnEnable()
    {
        if (missionTimer != null)
            missionTimer.OnTimeRatioChanged += OnTimeRatioChanged;
    }

    void OnDisable()
    {
        if (missionTimer != null)
            missionTimer.OnTimeRatioChanged -= OnTimeRatioChanged;
    }

    private void OnTimeRatioChanged(float ratio)
    {
        if (vitalityBar != null)
        {
            // Pasamos el ratio y el tiempo restante para que la barra decida su color
            vitalityBar.UpdateVisuals(ratio, missionTimer.CurrentTime);
        }

        ActualizarTextos(ratio);
    }

    private void ActualizarTextos(float ratio)
    {
        if (statusText == null || diagnosticText == null) return;

        if (ratio <= 0.25f)
        {
            statusText.text = "CRÍTICO";
            diagnosticText.text = "SISTEMA: FALLA INMINENTE";
            vibraText.text = "VIBRA CRÍTICA";
            statusText.color = redColor;
        }
        else if (ratio <= 0.6f)
        {
            statusText.text = "INESTABLE";
            diagnosticText.text = "ENERGÍA AL 50%";
            vibraText.text = "";
            statusText.color = Color.yellow;
        }
        else
        {
            statusText.text = "NOMINAL";
            diagnosticText.text = "INTEGRIDAD ÓPTIMA";
            vibraText.text = "";
            statusText.color = Color.cyan;
        }
    }
}
