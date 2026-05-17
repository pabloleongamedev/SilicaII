/*
 * Arquitectura: HUD/UI
 * Script: HUDManager
 * Rol: Agrupador visual legacy del HUD. No lee Health, Movement ni sistemas de gameplay.
 * Modulo: Mantiene referencias a presenters especializados para facilitar activar/desactivar el HUD como bloque.
 * Relaciones: HealthHUDPresenter observa HealthBehaviour; JetpackHUDPresenter observa IJetpackFuelReader.
 * Fase desacople: la logica de cada indicador vive en su presenter; este componente queda como composition helper de UI.
 * Uso como referencia: un manager de UI no debe consultar estado de gameplay ni buscar objetos globalmente.
 */
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [Header("Presenters")]
    [SerializeField] private HealthHUDPresenter healthPresenter;
    [SerializeField] private JetpackHUDPresenter jetpackPresenter;

    private void Awake()
    {
        if (healthPresenter == null)
            healthPresenter = GetComponentInChildren<HealthHUDPresenter>(true);

        if (jetpackPresenter == null)
            jetpackPresenter = GetComponentInChildren<JetpackHUDPresenter>(true);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
