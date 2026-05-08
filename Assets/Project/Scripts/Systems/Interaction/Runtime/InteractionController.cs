/*
 * Arquitectura: Interaction/Runtime
 * Script: InteractionController
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona deteccion, contexto y ejecucion de interacciones del jugador con objetos del mundo.
 * Relaciones: Usa IInteractable e InteractionContext para conectar jugador, mundo e Inventory sin dependencias profundas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[Obsolete("Usar PlayerInputHandler como entrada unica de interaccion. Este componente queda solo para compatibilidad de escenas antiguas.")]
public class InteractionController : MonoBehaviour
{
    [SerializeField] private InteractionDetector detector;
    [SerializeField] private InventoryController inventoryController;

    private InteractionContext context;

    private void Start()
    {
        if (inventoryController == null)
        {
            Debug.LogError("InventoryController no asignado");
            return;
        }

        context = new InteractionContext(inventoryController.ReadModel, inventoryController.WriteModel);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        var interactable = detector.CurrentInteractable;

        if (interactable != null)
        {
            interactable.Interact(context);
        }
    }
}
