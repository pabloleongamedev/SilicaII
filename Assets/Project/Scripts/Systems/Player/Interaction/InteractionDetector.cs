using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detecta objetos interactuables usando triggers.
/// Selecciona el mejor candidato según distancia y ángulo.
/// </summary>
public class InteractionDetector : MonoBehaviour
{
    private readonly List<IInteractable> interactables = new();

    public IInteractable CurrentInteractable { get; private set; }
    
    /// <summary>
    /// Evento cuando cambia el interactuable actual
    /// </summary>
    public System.Action<IInteractable> OnInteractableChanged;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;

    [Header("Settings")]
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float maxAngle = 60f;

    private float cosAngleThreshold;

    private void Awake()
    {
        if (playerTransform == null)
            playerTransform = transform;
        // Precalcular coseno para optimizar comparación
        cosAngleThreshold = Mathf.Cos(maxAngle * Mathf.Deg2Rad);
    }

    private void Update()
    {
        CleanInvalidInteractables();
        EvaluateBestInteractable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            if (!interactables.Contains(interactable))
                interactables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            interactables.Remove(interactable);
        }
    }
    /// <summary>
    /// Limpia referencias inválidas
    /// </summary>
    private void CleanInvalidInteractables()
    {
        interactables.RemoveAll(i =>
        {
            if (i == null) return true;

            var mb = i as MonoBehaviour;
            return mb == null || !mb.gameObject.activeInHierarchy;
        });
    }

    /// <summary>
    /// Selecciona el mejor interactuable
    /// </summary>

    private void EvaluateBestInteractable()
    {
        IInteractable best = null;
        float bestDistance = float.MaxValue;

        foreach (var interactable in interactables)
        {
            var mb = interactable as MonoBehaviour;
            if (mb == null) continue;

            Vector3 targetPos = mb.transform.position;

            float distance = Vector3.Distance(playerTransform.position, targetPos);
            if (distance > maxDistance) continue;

            if (cameraTransform != null)
            {
                Vector3 dir = (targetPos - cameraTransform.position).normalized;
                float dot = Vector3.Dot(cameraTransform.forward, dir);

                if (dot < cosAngleThreshold) continue;
            }

            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = interactable;
            }
        }

        if (CurrentInteractable != best)
        {
            CurrentInteractable = best;
            OnInteractableChanged?.Invoke(CurrentInteractable);
        }
    }
    public void ForceClear()
    {
        // Elimina SOLO el actual (más preciso que limpiar todo)
        if (CurrentInteractable != null)
        {
            interactables.Remove(CurrentInteractable);
        }

        CurrentInteractable = null;
        OnInteractableChanged?.Invoke(null);
    }
    public void ForceRefresh()
    {
        CleanInvalidInteractables();
        EvaluateBestInteractable();
    }
}