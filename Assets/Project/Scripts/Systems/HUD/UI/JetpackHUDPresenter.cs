/*
 * Arquitectura: HUD/UI
 * Script: JetpackHUDPresenter
 * Rol: Presenter dedicado del propulsor; observa IJetpackFuelReader y actualiza JetpackBarSegments/textos.
 * Relaciones: UI consume un contrato de lectura de combustible sin conocer JetpackSystem, MovementController ni input.
 * Fase 4: cada widget HUD tiene ownership claro y no depende de un manager central.
 */
using TMPro;
using UnityEngine;

public class JetpackHUDPresenter : MonoBehaviour
{
    [SerializeField] private MonoBehaviour fuelReaderBehaviour;
    [SerializeField] private JetpackBarSegments jetpackBar;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text diagnosticText;
    [SerializeField] private TMP_Text warningText;

    private readonly Color redColor = new Color(1f, 180f / 255f, 171f / 255f, 1f);
    private IJetpackFuelReader fuelReader;

    private void Awake()
    {
        fuelReader = ResolveFuelReader(fuelReaderBehaviour);

        if (jetpackBar == null)
            Debug.LogWarning("[JetpackHUDPresenter] Asigna JetpackBarSegments por Inspector.", this);
    }

    private void OnEnable()
    {
        ResolveFallbackFuelReader();

        if (fuelReader != null)
        {
            fuelReader.OnFuelRatioChanged += Render;
            Render(fuelReader.GetFuelRatio());
        }
    }

    private void OnDisable()
    {
        if (fuelReader != null)
            fuelReader.OnFuelRatioChanged -= Render;
    }

    private void Update()
    {
        // Compatibilidad con implementaciones que aun no emiten evento cada tick.
        if (fuelReader != null)
            Render(fuelReader.GetFuelRatio());
    }

    private void Render(float ratio)
    {
        if (jetpackBar != null)
            jetpackBar.UpdateVisuals(ratio);

        if (diagnosticText == null)
            return;

        if (ratio <= 0.25f)
        {
            SetStatus("PROPULSOR", redColor);
            diagnosticText.text = "EMPUJE_ACTIVO: CRITICO";
            diagnosticText.color = redColor;
            SetWarning("ENERGIA_CRITICA");
        }
        else if (ratio <= 0.6f)
        {
            SetStatus("PROPULSOR", Color.yellow);
            diagnosticText.text = "EMPUJE_ACTIVO: INESTABLE";
            diagnosticText.color = Color.yellow;
            SetWarning("");
        }
        else
        {
            SetStatus("PROPULSOR", Color.cyan);
            diagnosticText.text = "EMPUJE_ACTIVO: ESTABLE";
            diagnosticText.color = Color.cyan;
            SetWarning("");
        }
    }

    private void SetStatus(string value, Color color)
    {
        if (statusText == null)
            return;

        statusText.text = value;
        statusText.color = color;
    }

    private void SetWarning(string value)
    {
        if (warningText != null)
            warningText.text = value;
    }

    private void ResolveFallbackFuelReader()
    {
        if (fuelReader != null)
            return;

        fuelReader = ResolveFuelReader(fuelReaderBehaviour);

        if (fuelReader == null)
            Debug.LogWarning("[JetpackHUDPresenter] Asigna un MonoBehaviour que implemente IJetpackFuelReader por Inspector.", this);
    }

    private IJetpackFuelReader ResolveFuelReader(MonoBehaviour candidate)
    {
        if (candidate == null)
            return null;

        if (candidate is IJetpackFuelReader directReader)
            return directReader;

        return candidate.GetComponent<IJetpackFuelReader>();
    }
}
