/*
 * Arquitectura: Menu/UI
 * Script: ButtonAccessibility
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Se conecta con Menu/Settings para iniciar, cargar y configurar partida.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAccessibility : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISubmitHandler
{
    private Outline _outline;
    private Vector3 _initialScale;

    [SerializeField] private float _hoverScale = 1.05f;
    [SerializeField] private MonoBehaviour audioServiceBehaviour;

    private IAudioService audioService;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _initialScale = transform.localScale;

        if (_outline != null)
            _outline.enabled = false;

        ResolveAudioService();
    }

    public void OnSelect(BaseEventData eventData)
    {
        DoHoverEffects(true);
        Play(AudioCueKey.UIHover);
    }

    public void OnDeselect(BaseEventData eventData) => DoHoverEffects(false);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnPointerClick(PointerEventData eventData) => Play(AudioCueKey.UIClick);
    public void OnSubmit(BaseEventData eventData) => Play(AudioCueKey.UIClick);

    private void DoHoverEffects(bool isActive)
    {
        if (_outline != null)
            _outline.enabled = isActive;

        transform.localScale = isActive ? _initialScale * _hoverScale : _initialScale;
    }

    private void Play(AudioCueKey key) => audioService?.PlayOneShot(key);

    private void ResolveAudioService()
    {
        audioService = audioServiceBehaviour as IAudioService;

        if (audioService == null && audioServiceBehaviour != null)
            Debug.LogWarning("[ButtonAccessibility] El Audio Service asignado no implementa IAudioService.", this);
    }
}
