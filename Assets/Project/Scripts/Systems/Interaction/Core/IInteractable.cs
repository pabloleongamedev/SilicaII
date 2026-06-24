/*
 * Arquitectura: Interaction/Core
 * Script: IInteractable
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona deteccion, contexto y ejecucion de interacciones del jugador con objetos del mundo.
 * Relaciones: Usa IInteractable e InteractionContext para conectar jugador, mundo e Inventory sin dependencias profundas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Se ejecuta cuando el jugador interactúa (E)
    /// </summary>
    void Interact(InteractionContext context);

    /// <summary>
    /// Texto que se muestra en UI (ej: "Presiona E para abrir")
    /// </summary>
    string GetInteractionText();
}