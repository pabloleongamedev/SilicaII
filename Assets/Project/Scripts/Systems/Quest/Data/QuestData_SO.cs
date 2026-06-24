/*
 * Arquitectura: Quest/Data
 * Script: QuestData_SO
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona misiones y progreso a partir de eventos de gameplay como recolectar, refinar o craftear.
 * Relaciones: Las tareas siguen referenciando ItemData_SO para authoring en Inspector, pero Runtime compara por itemID.
 * Fase 5: mantiene usabilidad de assets y desacopla el payload de eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Quest/Quest")]
public class QuestData_SO : ScriptableObject
{
    public string questName;
    public List<QuestTask> tasks;
}

[System.Serializable]
public class QuestTask
{
    public string description;
    public QuestTaskType type;
    public ItemData_SO targetItem;
    public int requiredAmount;
}
