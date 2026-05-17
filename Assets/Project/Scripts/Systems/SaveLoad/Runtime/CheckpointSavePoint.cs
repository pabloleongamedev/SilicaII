/*
 * Arquitectura: SaveLoad/Runtime
 * Script: CheckpointSavePoint
 * Rol: Interactable runtime de guardado manual. Puede usarse con sistema de interaccion, trigger en mundo o boton UI.
 * Relaciones: Implementa IInteractable; consume ISaveCheckpointUseCase asignable por Inspector.
 * Riesgo arquitectonico mitigado: no conoce GameManager; requiere asignar un adapter por Inspector.
 * Uso como referencia: separa la intencion "guardar checkpoint" del flujo automatico de autosave y la expone al sistema de interaccion.
 */
using UnityEngine;

public class CheckpointSavePoint : MonoBehaviour, IInteractable
{
    [Header("Trigger")]
    [SerializeField] private bool saveOnTriggerEnter = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float cooldownSeconds = 1f;

    [Header("Interaction")]
    [SerializeField] private string interactionText = "Presiona E para guardar checkpoint";

    [Header("Save Slot")]
    [SerializeField] private MonoBehaviour saveCheckpointUseCaseBehaviour;
    [SerializeField] private bool createSaveDataIfMissing = true;

    private float lastSaveTime = -999f;
    private ISaveCheckpointUseCase saveCheckpointUseCase;

    private void Awake()
    {
        saveCheckpointUseCase = saveCheckpointUseCaseBehaviour as ISaveCheckpointUseCase;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!saveOnTriggerEnter)
            return;

        if (!other.CompareTag(playerTag))
            return;

        SaveCheckpoint();
    }

    public void Interact(InteractionContext context)
    {
        SaveCheckpoint();
    }

    public string GetInteractionText()
    {
        return interactionText;
    }

    public void SaveCheckpoint()
    {
        if (Time.time - lastSaveTime < cooldownSeconds)
            return;

        if (saveCheckpointUseCase == null)
        {
            Debug.LogWarning("[CheckpointSavePoint] No existe ISaveCheckpointUseCase activo para guardar checkpoint.", this);
            return;
        }

        bool saved = saveCheckpointUseCase.TrySaveCheckpoint(createSaveDataIfMissing);

        if (!saved)
        {
            Debug.LogWarning("[CheckpointSavePoint] No se pudo guardar el checkpoint. Revisa los logs de GameManager/SaveController.", this);
            return;
        }

        lastSaveTime = Time.time;
        Debug.Log($"[CheckpointSavePoint] Checkpoint guardado en slot {saveCheckpointUseCase.CurrentSlotID}.", this);
    }
}
