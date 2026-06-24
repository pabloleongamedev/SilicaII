/*
 * Arquitectura: SaveLoad/Runtime
 * Script: CheckpointRestorePoint
 * Rol: Interactable runtime de restauracion manual. Puede usarse con sistema de interaccion, trigger en mundo o boton UI.
 * Relaciones: Implementa IInteractable; consume IRestoreCheckpointUseCase por Inspector mediante SaveLoadSceneBinding u otro provider.
 * Riesgo arquitectonico mitigado: no conoce una fachada global; habla con un contrato de caso de uso expuesto por SaveLoadSceneBinding.
 * Uso como referencia: separa la intencion "restaurar checkpoint" del guardado automatico y la expone al sistema de interaccion.
 */
using UnityEngine;

public class CheckpointRestorePoint : MonoBehaviour, IInteractable
{
    [Header("Trigger")]
    [SerializeField] private bool restoreOnTriggerEnter = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float cooldownSeconds = 1f;

    [Header("Slot")]
    [SerializeField] private MonoBehaviour restoreCheckpointUseCaseBehaviour;
    [SerializeField] private bool useCurrentSlot = true;
    [SerializeField] private string fallbackSlotID = "1";
    [SerializeField] private bool reloadScene = false;

    [Header("Interaction")]
    [SerializeField] private string interactionText = "Presiona E para restaurar checkpoint";

    private float lastRestoreTime = -999f;
    private IRestoreCheckpointUseCase restoreCheckpointUseCase;

    private void Awake()
    {
        restoreCheckpointUseCase = restoreCheckpointUseCaseBehaviour as IRestoreCheckpointUseCase;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!restoreOnTriggerEnter)
            return;

        if (!other.CompareTag(playerTag))
            return;

        RestoreCheckpoint();
    }

    public void Interact(InteractionContext context)
    {
        RestoreCheckpoint();
    }

    public string GetInteractionText()
    {
        return interactionText;
    }

    public void RestoreCheckpoint()
    {
        if (Time.time - lastRestoreTime < cooldownSeconds)
            return;

        ResolveUseCase();

        if (restoreCheckpointUseCase == null)
        {
            Debug.LogWarning("[CheckpointRestorePoint] No existe IRestoreCheckpointUseCase activo para restaurar checkpoint.", this);
            return;
        }

        string slotID = useCurrentSlot
            ? restoreCheckpointUseCase.CurrentSlotID
            : fallbackSlotID;

        if (string.IsNullOrEmpty(slotID))
            slotID = fallbackSlotID;

        if (!restoreCheckpointUseCase.HasCheckpoint(slotID))
        {
            Debug.LogWarning($"[CheckpointRestorePoint] No existe guardado para el slot {slotID}.", this);
            return;
        }

        lastRestoreTime = Time.time;
        Debug.Log($"[CheckpointRestorePoint] Restaurando checkpoint desde slot {slotID}.", this);
        restoreCheckpointUseCase.RestoreCheckpoint(slotID, reloadScene);
    }

    private void ResolveUseCase()
    {
        if (restoreCheckpointUseCase == null)
            restoreCheckpointUseCase = restoreCheckpointUseCaseBehaviour as IRestoreCheckpointUseCase;
    }
}
