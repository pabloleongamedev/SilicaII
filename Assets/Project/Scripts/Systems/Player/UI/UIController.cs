/*
 * Arquitectura: Player/UI
 * Script: UIController
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona estado global del jugador, input y bloqueos de gameplay/UI.
 * Relaciones: Coordina input, estados UI/gameplay y bloqueos globales usados por otros sistemas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject crosshair;

    private void OnEnable()
    {
        GameplayEvents.OnUIStateChanged += HandleState;
    }

    private void OnDisable()
    {
        GameplayEvents.OnUIStateChanged -= HandleState;
    }

    private void HandleState(UIState state)
    {
        if (crosshair == null) return;

        // 🔥 visible SOLO cuando no hay UI
        crosshair.SetActive(state == UIState.None);
    }
}