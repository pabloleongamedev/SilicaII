/*
 * Arquitectura: HUD/UI
 * Script: HealthBarSegments
 * Rol: Presenta informacion de salud recibida desde Runtime; no calcula dano ni estado de juego.
 * Modulo: Gestiona segmentos visuales de vida.
 * Relaciones: HealthHUDPresenter puede pasarle HealthBehaviour.HealthRatio, o la barra puede escuchar HealthBehaviour asignado por Inspector.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarSegments : MonoBehaviour
{
    [Header("Fuente Opcional")]
    [SerializeField] private HealthBehaviour health;

    [Header("Configuracion Visual")]
    [SerializeField] private List<Image> segments = new List<Image>();
    [SerializeField] private Color colorTop = new Color(0f, 1f, 1f, 1f);
    [SerializeField] private Color colorBottom = new Color(1f, 0.7058824f, 0.67058825f, 1f);
    [SerializeField] private Color emptyColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    [SerializeField] private float blinkSpeed = 8f;
    [SerializeField] private float damageBlinkDuration = 0.6f;

    private readonly Color redColor = new Color(1f, 180f / 255f, 171f / 255f, 1f);
    private float currentHealthRatio = 1f;
    private float damageBlinkUntil;

    private void Awake()
    {
        BindHealthIfAvailable();
    }

    private void OnEnable()
    {
        BindHealthIfAvailable();
        Render(currentHealthRatio);
    }

    private void OnDisable()
    {
        UnbindHealth();
    }

    private void Update()
    {
        // El blink es feedback visual temporal de dano; el estado canonico sigue viniendo de Health.
        Render(currentHealthRatio);
    }

    public void UpdateVisuals(float healthRatio)
    {
        currentHealthRatio = Mathf.Clamp01(healthRatio);
        Render(currentHealthRatio);
    }

    public void TriggerDamageBlink()
    {
        damageBlinkUntil = Time.time + damageBlinkDuration;
    }

    private void BindHealthIfAvailable()
    {
        if (health == null)
            return;

        health.OnHealthChanged -= HandleHealthChanged;
        health.OnDamaged -= HandleDamaged;
        health.OnHealthChanged += HandleHealthChanged;
        health.OnDamaged += HandleDamaged;
        UpdateVisuals(health.HealthRatio);
    }

    private void UnbindHealth()
    {
        if (health == null)
            return;

        health.OnHealthChanged -= HandleHealthChanged;
        health.OnDamaged -= HandleDamaged;
    }

    private void HandleHealthChanged(int current, int max)
    {
        UpdateVisuals(max <= 0 ? 0f : Mathf.Clamp01((float)current / max));
    }

    private void HandleDamaged(DamageContext context)
    {
        TriggerDamageBlink();
    }

    private void Render(float healthRatio)
    {
        int totalSegments = segments.Count;
        if (totalSegments == 0)
            return;

        int activeSegmentsCount = healthRatio > 0f
            ? Mathf.Max(1, Mathf.CeilToInt(healthRatio * totalSegments))
            : 0;
        bool shouldBlink = Time.time < damageBlinkUntil;
        float blink = shouldBlink ? Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed)) : 0f;

        for (int i = 0; i < totalSegments; i++)
        {
            bool isSegmentActive = i < activeSegmentsCount;

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
}
