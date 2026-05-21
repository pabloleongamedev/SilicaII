/*
 * Arquitectura: HUD/UI
 * Script: HealthHUDPresenter
 * Rol: Presenter dedicado de la vida; observa HealthBehaviour y actualiza HealthBarSegments/textos.
 * Relaciones: UI consume eventos de HealthBehaviour asignado por Inspector sin que Health conozca UI.
 * Fase 4: divide el HUD por indicadores para evitar un controlador central que conozca todos los sistemas.
 */
using TMPro;
using UnityEngine;

public class HealthHUDPresenter : MonoBehaviour
{
    [SerializeField] private HealthBehaviour health;
    [SerializeField] private HealthBarSegments healthBar;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text diagnosticText;
    [SerializeField] private TMP_Text warningText;

    private readonly Color redColor = new Color(1f, 180f / 255f, 171f / 255f, 1f);

    private void Awake()
    {
        if (health == null)
            Debug.LogWarning("[HealthHUDPresenter] Asigna HealthBehaviour por Inspector.", this);

        if (healthBar == null)
            Debug.LogWarning("[HealthHUDPresenter] Asigna HealthBarSegments por Inspector.", this);
    }

    private void OnEnable()
    {
        if (health == null)
        {
            Debug.LogWarning("[HealthHUDPresenter] Asigna HealthBehaviour por Inspector para conectar HUD con Health.", this);
            return;
        }

        health.OnHealthChanged += HandleHealthChanged;
        health.OnDamaged += HandleDamaged;
        Render(health.HealthRatio);
    }

    private void OnDisable()
    {
        if (health == null)
            return;

        health.OnHealthChanged -= HandleHealthChanged;
        health.OnDamaged -= HandleDamaged;
    }

    private void HandleHealthChanged(int current, int max)
    {
        Render(max <= 0 ? 0f : Mathf.Clamp01((float)current / max));
    }

    private void HandleDamaged(DamageContext context)
    {
        if (healthBar != null)
            healthBar.TriggerDamageBlink();
    }

    private void Render(float ratio)
    {
        if (healthBar != null)
            healthBar.UpdateVisuals(ratio);

        if (statusText == null || diagnosticText == null)
            return;

        if (ratio <= 0.25f)
        {
            statusText.text = "CRITICO";
            diagnosticText.text = "SISTEMA: FALLA INMINENTE";
            statusText.color = redColor;

            if (warningText != null)
                warningText.text = "VIBRA CRITICA";
        }
        else if (ratio <= 0.6f)
        {
            statusText.text = "INESTABLE";
            diagnosticText.text = "ENERGIA AL 50%";
            statusText.color = Color.yellow;

            if (warningText != null)
                warningText.text = "";
        }
        else
        {
            statusText.text = "NOMINAL";
            diagnosticText.text = "INTEGRIDAD OPTIMA";
            statusText.color = Color.cyan;

            if (warningText != null)
                warningText.text = "";
        }
    }
}
