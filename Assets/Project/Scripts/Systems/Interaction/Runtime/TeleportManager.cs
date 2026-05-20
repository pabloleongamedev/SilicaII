/*
 * Arquitectura: Interaction/Runtime
 * Script: TeleportManager
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona deteccion, contexto y ejecucion de interacciones del jugador con objetos del mundo.
 * Relaciones: Usa IInteractable e InteractionContext para conectar jugador, mundo e Inventory sin dependencias profundas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class TeleportManager : MonoBehaviour, IInteractable
{
    [Header("Targets")]
    [SerializeField] private Transform spaceShipHall;
    [SerializeField] private Transform exteriorMap;
    [SerializeField] private GameObject player;

    [Header("Refs")]
    [SerializeField] private QuestSystem questSystem;
    [SerializeField] private QuestEventChannel_SO questChannel;
    [SerializeField] private NotificationEventChannel_SO notificationChannel;
    private bool canExit = false;

    // =========================================================
    public void Interact(InteractionContext context)
    {
        // VALIDACIÓN ANTES DE TELEPORT
        if (!CanExit(out string message))
        {
            Notify(message, NotificationType.Error);
            return;
        }

        TeleportPlayer(player);
    }

    public string GetInteractionText()
    {
        return "Presiona E para abrir la nave";
    }

    // =========================================================
    // 🔥 VALIDACIÓN DE MISIONES
    // =========================================================

    private bool CanExit(out string message)
    {
        message = "";

        if (questSystem == null)
        {
            message = "Sistema de misiones no encontrado";
            return false;
        }

        var quest = questSystem.GetCurrentQuest();

        if (quest == null || quest.tasks == null)
        {
            message = "No hay misión activa";
            return false;
        }

        // 🔥 VALIDAMOS LAS PRIMERAS 2 TAREAS
        int tasksToCheck = Mathf.Min(2, quest.tasks.Count);

        for (int i = 0; i < tasksToCheck; i++)
        {
            if (!canExit)
            {
                // 🔥 MENSAJE DINÁMICO
                message = $"Debes completar: {quest.tasks[i].description}";
                return false;
            }
        }

        return true;
    }
    

    private void OnEnable()
    {
        if (questChannel != null)
            questChannel.QuestCompleted += HandleQuestCompleted;
    }

    private void OnDisable()
    {
        if (questChannel != null)
            questChannel.QuestCompleted -= HandleQuestCompleted;
    }

    private void HandleQuestCompleted(int questIndex)
    {
        
        if (questIndex >= 1)
        {
            
            canExit = true;
        }
    }

    // =========================================================
    private void TeleportPlayer(GameObject player)
    {
        Debug.Log("[Teleport] Ejecutando teletransporte");

        Vector3 playerPos = player.transform.position;

        float distToShip = Vector3.Distance(playerPos, spaceShipHall.position);
        float distToExterior = Vector3.Distance(playerPos, exteriorMap.position);

        Vector3 endPoint = distToShip < distToExterior
            ? exteriorMap.position
            : spaceShipHall.position;

        var controller = player.GetComponent<CharacterController>();
        var rigidBody = player.GetComponent<Rigidbody>();

        if (controller != null)
            controller.enabled = false;

        if (rigidBody != null)
        {
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.isKinematic = true;
        }

        player.transform.position = endPoint;

        Physics.SyncTransforms();

        if (controller != null)
            controller.enabled = true;

        if (rigidBody != null)
            rigidBody.isKinematic = false;
    }

    // =========================================================
    private void Notify(string msg, NotificationType type)
    {
        Debug.Log("[Teleport] " + msg);

        notificationChannel?.Raise(new NotificationData
        {
            message = msg,
            type = type
        });
    }
}
