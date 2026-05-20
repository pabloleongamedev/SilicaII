/*
 * Arquitectura: Menu/UI
 * Script: ButtonAccessibility
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Se conecta con Menu/Settings para iniciar, cargar y configurar partida.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAccessibility : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Outline _outline;
    private Vector3 _initialScale;
    [SerializeField] private float _hoverScale = 1.05f;

    void Awake()
    {
        _outline = GetComponent<Outline>();
        _initialScale = transform.localScale;
        if (_outline != null) _outline.enabled = false;
    }

    // --- SELECCIÓN (Teclado / Mando) ---
    public void OnSelect(BaseEventData eventData) => DoHoverEffects(true);
    public void OnDeselect(BaseEventData eventData) => DoHoverEffects(false);

    // --- HOVER (Mouse) ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Al entrar con el mouse, le decimos al sistema que este es el objeto seleccionado
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // Al salir con el mouse, quitamos la selección si no hay otro mando activo
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void DoHoverEffects(bool isActive)
    {
        if (_outline != null) _outline.enabled = isActive;
        transform.localScale = isActive ? _initialScale * _hoverScale : _initialScale;
    }
}
