/*
 * Arquitectura: HUD/UI
 * Script: JetpackBarSegments
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona indicadores visuales persistentes de estado, tiempo, vitalidad y propulsor.
 * Relaciones: Presenta datos de runtime sin ser la fuente de reglas de gameplay.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class JetpackBarSegments : MonoBehaviour
{
    [Header("Configuración Visual")]
    [SerializeField] private List<Image> segments = new List<Image>();
    [SerializeField] private Color colorTop = new Color(0f, 1f, 1f, 1f);
    [SerializeField] private Color colorBottom = new Color(1f, 0.7058824f, 0.67058825f, 1f);
    [SerializeField] private Color emptyColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    Color redColor = new Color(1f, 180f/255f, 171f/255f, 1f);

    public void UpdateVisuals(float ratio, float secondsRemaining)
    {
        int totalSegments = segments.Count;
        if (totalSegments == 0) return;

        int activeSegmentsCount = Mathf.RoundToInt(ratio * totalSegments);

        for (int i = 0; i < totalSegments; i++)
        {
            bool isSegmentActive = i < activeSegmentsCount;

            if (isSegmentActive)
            {
                float t = (float)i / (float)(totalSegments - 1);
                segments[i].color = Color.Lerp(colorBottom, colorTop, t);
                
                if (secondsRemaining <= 600f)
                {
                    float blink = Mathf.Abs(Mathf.Sin(Time.time * 8f));
                    segments[i].color = Color.Lerp(segments[i].color, redColor, blink);
                }
            }
            else
            {
                segments[i].color = emptyColor;
            }
        }
    }
}
