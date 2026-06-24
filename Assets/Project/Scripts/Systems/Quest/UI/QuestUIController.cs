/*
 * Arquitectura: Quest/UI
 * Script: QuestUIController
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona misiones y progreso a partir de eventos de gameplay como recolectar, refinar o craftear.
 * Relaciones: Escucha QuestEventChannel_SO para renderizar snapshots y consulta QuestSystem asignado por Inspector para reconstruir progreso al activarse.
 * Riesgo arquitectonico mitigado: elimina busqueda global de QuestSystem; la dependencia de lectura queda visible en escena.
 */
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestUIController : MonoBehaviour
{
    [Header("Mission")]
    [SerializeField] private Transform missionContent;
    [SerializeField] private GameObject missionPrefab;

    [Header("Tasks")]
    [SerializeField] private Transform taskContent;
    [SerializeField] private GameObject taskPrefab;

    // 🔥 TRACK REAL POR MISIÓN
    private class QuestUIBlock
    {
        public GameObject missionGO;
        public int startIndex;
        public int taskCount;
    }

    private List<QuestUIBlock> questBlocks = new();

    [SerializeField] private QuestSystem questSystemRef;
    [SerializeField] private QuestEventChannel_SO questChannel;

    // =========================================================
    private void Awake()
    {
        if (questChannel != null)
        {
            questChannel.QuestLoaded += BuildUI;
            questChannel.TaskUpdated += UpdateTask;
        }
    }

    private void OnDestroy()
    {
        if (questChannel != null)
        {
            questChannel.QuestLoaded -= BuildUI;
            questChannel.TaskUpdated -= UpdateTask;
        }
    }

    private void OnEnable()
    {
        if (questSystemRef != null)
        {
            var quest = questSystemRef.GetCurrentQuest();

            if (quest != null)
            {
                if (taskContent.childCount == 0)
                {
                    BuildUI(quest);
                }

                RebuildProgress(questSystemRef, quest);
            }
        }
    }

    // =========================================================
    // 🔥 CREA BLOQUE COMPLETO DE MISIÓN
    // =========================================================
    private void BuildUI(QuestData_SO quest)
    {
        Debug.Log("BUILD UI: " + quest.questName);

        var block = new QuestUIBlock();

        // 🔥 MISION
        var missionGO = Instantiate(missionPrefab, missionContent);
        missionGO.transform.SetSiblingIndex(0);

        var missionText = missionGO.GetComponentInChildren<TMP_Text>();
        missionText.text = quest.questName;

        block.missionGO = missionGO;

        // 🔥 GUARDAR DONDE EMPIEZAN SUS TASKS
        block.startIndex = taskContent.childCount;
        block.taskCount = quest.tasks.Count;

        // 🔥 TASKS
        foreach (var task in quest.tasks)
        {
            var go = Instantiate(taskPrefab, taskContent);

            var texts = go.GetComponentsInChildren<TMP_Text>();
            texts[0].text = task.description;
            texts[1].text = $"0 / {task.requiredAmount}";
        }

        questBlocks.Add(block);
    }

    // =========================================================
    private void UpdateTask(int index, int current, int required, bool completed)
    {
        if (questBlocks.Count == 0)
            return;

        // 🔥 SIEMPRE ACTUALIZAMOS LA ÚLTIMA MISIÓN ACTIVA
        var block = questBlocks[questBlocks.Count - 1];

        int realIndex = block.startIndex + index;

        if (realIndex < 0 || realIndex >= taskContent.childCount)
            return;

        var go = taskContent.GetChild(realIndex);
        var texts = go.GetComponentsInChildren<TMP_Text>();

        texts[1].text = $"{current} / {required}";

        if (completed)
        {
            texts[0].color = Color.gray;
            texts[1].color = Color.gray;
        }

        CheckMissionCompleted(block);
    }

    // =========================================================
    private void RebuildProgress(QuestSystem questSystem, QuestData_SO quest)
    {
        for (int i = 0; i < quest.tasks.Count; i++)
        {
            int current = questSystem.GetTaskProgress(i);
            int required = quest.tasks[i].requiredAmount;

            bool completed = current >= required;

            UpdateTask(i, current, required, completed);
        }
    }

    // =========================================================
    private void CheckMissionCompleted(QuestUIBlock block)
    {
        if (questSystemRef == null)
            return;

        var quest = questSystemRef.GetCurrentQuest();
        if (quest == null)
            return;

        for (int i = 0; i < quest.tasks.Count; i++)
        {
            int current = questSystemRef.GetTaskProgress(i);
            int required = quest.tasks[i].requiredAmount;

            if (current < required)
                return;
        }

        var text = block.missionGO.GetComponentInChildren<TMP_Text>();
        text.color = Color.gray;
    }
}
