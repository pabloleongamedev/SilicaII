/*
 * Arquitectura: Quest/Data
 * Script: QuestTaskType
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona misiones y progreso a partir de eventos de gameplay como recolectar, refinar o craftear.
 * Relaciones: Escucha eventos de Inventory/Crafting y publica estado de mision hacia UI u otros sistemas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
public enum QuestTaskType
{
    Collect,
    Refine,
    Craft
}