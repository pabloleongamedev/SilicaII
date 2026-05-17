/*
 * Arquitectura: Quest/Data
 * Script: QuestTaskData_SO
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona misiones y progreso a partir de eventos de gameplay como recolectar, refinar o craftear.
 * Relaciones: Usa ItemData_SO para authoring; Quest runtime convierte la comparacion a itemID.
 * Fase 5: separa authoring de datos en Unity del payload runtime entre sistemas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Task")]
public class QuestTaskData_SO : ScriptableObject
{
    public QuestTaskType type;

    [Header("Target")]
    public ItemData_SO targetItem;
    public int requiredAmount = 1;
}
