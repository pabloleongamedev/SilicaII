/*
 * Arquitectura: Quest/Runtime
 * Script: QuestSystem
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona misiones y progreso a partir de eventos de gameplay como recolectar, refinar o craftear.
 * Relaciones: Escucha QuestEventChannel_SO con itemID y publica snapshots para UI/teleport.
 * Fase 5: Quest compara por itemID para no depender de la misma instancia ItemData_SO que Inventory/Crafting.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using System.Collections.Generic;

public class QuestSystem : MonoBehaviour
{
    [SerializeField] private List<QuestData_SO> quests;

    [Header("Events")]
    [SerializeField] private QuestEventChannel_SO questChannel;

    private int currentQuestIndex = 0;
    private QuestData_SO currentQuest;

    private Dictionary<int, int> progress = new();

    private bool isInitialized = false;

    private void Start()
    {
        LoadQuest(0);
    }

    private void OnEnable()
    {
        // Quest progresa reaccionando a eventos publicados por otros modulos.
        // Inventory/Crafting no necesitan conocer el estado interno de misiones.
        if (questChannel == null)
            return;

        questChannel.ItemCollected += HandleCollect;
        questChannel.ItemRefined += HandleRefined;
        questChannel.ItemCrafted += HandleCraft;
    }

    private void OnDisable()
    {
        if (questChannel == null)
            return;

        questChannel.ItemCollected -= HandleCollect;
        questChannel.ItemRefined -= HandleRefined;
        questChannel.ItemCrafted -= HandleCraft;
    }

    // =========================================
    private void LoadQuest(int index)
    {
        // QuestData_SO es el dato editable; el progreso runtime se inicializa aqui.
        // La UI se entera mediante QuestEventChannel_SO.QuestLoaded.
        if (index >= quests.Count)
        {
            Debug.Log("TODAS LAS MISIONES COMPLETADAS");
            return;
        }

        currentQuestIndex = index;
        currentQuest = quests[index];

        progress.Clear();

        for (int i = 0; i < currentQuest.tasks.Count; i++)
            progress[i] = 0;

        isInitialized = true;

        // 🔥 UI
        questChannel?.RaiseQuestLoaded(currentQuest);
    }

    // =========================================
    private void HandleCollect(string itemID, int amount)
    {
        if (!isInitialized) return;
        UpdateTasks(itemID, amount, QuestTaskType.Collect);
    }

    private void HandleRefined(string itemID, int amount)
    {
        if (!isInitialized) return;
        UpdateTasks(itemID, amount, QuestTaskType.Refine);
    }

    private void HandleCraft(string itemID, int amount)
    {
        if (!isInitialized) return;
        UpdateTasks(itemID, amount, QuestTaskType.Craft);
    }

    // =========================================
    private void UpdateTasks(string itemID, int amount, QuestTaskType type)
    {
        // Una tarea avanza si coinciden tipo e itemID objetivo.
        // Se emite un snapshot de progreso para mantener UI desacoplada.
        if (currentQuest == null) return;
        if (currentQuest.tasks == null) return;
        if (string.IsNullOrEmpty(itemID)) return;

        for (int i = 0; i < currentQuest.tasks.Count; i++)
        {
            var task = currentQuest.tasks[i];

            if (task == null || task.targetItem == null)
                continue;

            if (task.type != type) continue;
            if (task.targetItem.itemID != itemID) continue;

            int required = task.requiredAmount;
            progress[i] = Mathf.Min(progress[i] + amount, required);

            int current = progress[i];

            bool completed = current >= required;

            questChannel?.RaiseTaskUpdated(i, current, required, completed);
        }

        CheckQuestComplete();
    }
    private void CheckQuestComplete()
    {
        // Al completar se avisa por evento modular y luego se carga la siguiente.
        // QuestSystem no decide quien reacciona: solo emite el snapshot por canal.
        for (int i = 0; i < currentQuest.tasks.Count; i++)
        {
            if (progress[i] < currentQuest.tasks[i].requiredAmount)
                return;
        }

        Debug.Log("MISIÓN COMPLETADA");

        // 🔥 ESTE ES EL PUNTO CLAVE
        questChannel?.RaiseQuestCompleted(currentQuestIndex);

        LoadQuest(currentQuestIndex + 1);
    }
    public QuestData_SO GetCurrentQuest()
    {
        return currentQuest;
    }

    public int GetTaskProgress(int index)
    {
        if (progress.ContainsKey(index))
            return progress[index];

        return 0;
    }

}
