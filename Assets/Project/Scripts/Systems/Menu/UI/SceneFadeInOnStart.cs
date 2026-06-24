/*
 * Arquitectura: Menu/UI
 * Script: SceneFadeInOnStart
 * Rol: Transicion visual local de entrada a una escena.
 * Relaciones: Usa una Image/Graphic asignada por Inspector; no carga escenas ni conoce SaveLoad.
 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeInOnStart : MonoBehaviour
{
    [Header("Fade In")]
    [SerializeField] private Graphic fadeGraphic;
    [SerializeField, Min(0f)] private float fadeInDuration = 0.45f;

    private void OnEnable()
    {
        if (fadeGraphic == null)
        {
            Debug.LogWarning("[SceneFadeInOnStart] Asigna fadeGraphic por Inspector.", this);
            return;
        }

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        gameObject.SetActive(true);

        Color startColor = fadeGraphic.color;
        Color targetColor = startColor;
        targetColor.a = 0f;
        fadeGraphic.color = startColor;

        if (fadeInDuration <= 0f)
        {
            fadeGraphic.color = targetColor;
            gameObject.SetActive(false);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeInDuration);
            fadeGraphic.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        fadeGraphic.color = targetColor;
        gameObject.SetActive(false);
    }
}
