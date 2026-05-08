using UnityEngine;

public class TeleportManager : MonoBehaviour, IInteractable
{
    [Header("Targets")]
    [SerializeField] private Transform spaceShipHall;
    [SerializeField] private Transform exteriorMap;
    [SerializeField] private GameObject player;

    [Header("Refs")]
    [SerializeField] private QuestSystem questSystem;
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
        QuestEvents.OnQuestCompleted += HandleQuestCompleted;
    }

    private void OnDisable()
    {
        QuestEvents.OnQuestCompleted -= HandleQuestCompleted;
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

        GameplayEvents.OnNotification?.Invoke(new NotificationData
        {
            message = msg,
            type = type
        });
    }
}
