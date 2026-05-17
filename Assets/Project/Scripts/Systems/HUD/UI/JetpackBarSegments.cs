/*
 * Arquitectura: HUD/UI
 * Script: JetpackBarSegments
 * Rol: Presenta informacion de combustible recibida desde Movement Runtime.
 * Modulo: Gestiona segmentos visuales del jetpack.
 * Relaciones: HUDManager consulta MovementController.GetJetpackRatio y pasa el ratio a esta vista; la vista no modifica combustible ni input.
 * Nota de escena: si la lista Segments queda vacia en Inspector, se autocompleta con Image hijos para evitar una barra sin pintar.
 * Feedback: parpadea cuando el ratio baja, porque ese evento visual representa consumo de energia, no una regla del JetpackSystem.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class JetpackBarSegments : MonoBehaviour
{
    [Header("Configuracion Visual")]
    [SerializeField] private List<Image> segments = new List<Image>();
    [SerializeField] private Color colorTop = new Color(0f, 1f, 1f, 1f);
    [SerializeField] private Color colorBottom = new Color(1f, 0.7058824f, 0.67058825f, 1f);
    [SerializeField] private Color emptyColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    [SerializeField] private bool fillFromBottom = true;
    [SerializeField] private float blinkSpeed = 8f;
    [SerializeField] private float consumeBlinkDuration = 0.25f;

    private readonly Color redColor = new Color(1f, 180f / 255f, 171f / 255f, 1f);
    private float lastFuelRatio = 1f;
    private float consumeBlinkUntil;

    private void Awake()
    {
        AutoPopulateSegmentsIfNeeded();
    }

    public void UpdateVisuals(float fuelRatio)
    {
        AutoPopulateSegmentsIfNeeded();

        int totalSegments = segments.Count;
        if (totalSegments == 0)
            return;

        fuelRatio = Mathf.Clamp01(fuelRatio);

        // Si el ratio baja, la vista da feedback. El consumo real sigue viviendo en JetpackSystem.
        if (fuelRatio < lastFuelRatio - 0.001f)
            consumeBlinkUntil = Time.time + consumeBlinkDuration;

        lastFuelRatio = fuelRatio;

        int activeSegmentsCount = fuelRatio > 0f
            ? Mathf.Max(1, Mathf.CeilToInt(fuelRatio * totalSegments))
            : 0;
        bool shouldBlink = Time.time < consumeBlinkUntil;
        float blink = shouldBlink ? Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed)) : 0f;

        for (int i = 0; i < totalSegments; i++)
        {
            bool isSegmentActive = fillFromBottom
                ? i >= totalSegments - activeSegmentsCount
                : i < activeSegmentsCount;

            if (isSegmentActive)
            {
                float t = totalSegments <= 1 ? 1f : (float)i / (totalSegments - 1);
                segments[i].color = Color.Lerp(colorBottom, colorTop, t);

                if (shouldBlink)
                    segments[i].color = Color.Lerp(segments[i].color, redColor, blink);
            }
            else
            {
                segments[i].color = emptyColor;
            }
        }
    }

    private void AutoPopulateSegmentsIfNeeded()
    {
        if (segments.Count > 0)
            return;

        var childImages = GetComponentsInChildren<Image>(true);

        foreach (var image in childImages)
        {
            // El Image del mismo panel suele ser fondo; los segmentos viven en hijos.
            if (image != null && image.gameObject != gameObject)
                segments.Add(image);
        }
    }
}
