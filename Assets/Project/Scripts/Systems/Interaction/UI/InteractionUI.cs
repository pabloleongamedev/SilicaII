/*
 * Arquitectura: Interaction/UI
 * Script: InteractionUI
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona deteccion, contexto y ejecucion de interacciones del jugador con objetos del mundo.
 * Relaciones: Usa IInteractable e InteractionContext para conectar jugador, mundo e Inventory sin dependencias profundas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using TMPro;
using UnityEngine;
using System;

[Obsolete("Usar InteractionUIController. Este componente queda solo para compatibilidad con prefabs antiguos.")]
public class InteractionUI : MonoBehaviour
{
    [SerializeField] private InteractionDetector detector;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        var interactable = detector.CurrentInteractable;

        if (interactable != null)
        {
            panel.SetActive(true);
            text.text = interactable.GetInteractionText();
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
