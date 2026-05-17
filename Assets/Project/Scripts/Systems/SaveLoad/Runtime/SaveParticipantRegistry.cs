/*
 * Arquitectura: SaveLoad/Runtime
 * Script: SaveParticipantRegistry
 * Rol: Composition root local de participantes de guardado de una escena.
 * Relaciones: Recibe MonoBehaviours asignados por Inspector y tambien descubre participantes en el mismo GameObject/hijos.
 * Riesgo arquitectonico mitigado: reemplaza busquedas globales; se anuncia al GameManager cuando la escena lo activa.
 */
using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveParticipantRegistry : MonoBehaviour
{
    public static event Action<SaveParticipantRegistry> OnRegistryAvailable;

    [SerializeField] private ItemDatabase_SO itemDatabase;
    [SerializeField] private MonoBehaviour[] participantBehaviours;
    [SerializeField] private bool includeParticipantsOnThisObject = true;
    [SerializeField] private bool includeParticipantsInChildren = false;

    public ItemDatabase_SO ItemDatabase => itemDatabase;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        OnRegistryAvailable = null;
    }

    private void OnEnable()
    {
        OnRegistryAvailable?.Invoke(this);
    }

    private void Start()
    {
        // Reanuncia en Start por si el GameManager se suscribio despues del OnEnable de la escena.
        OnRegistryAvailable?.Invoke(this);
    }

    public void Capture(GameData gameData)
    {
        foreach (var participant in GetParticipants())
            participant.Capture(gameData);
    }

    public void Restore(GameData gameData, IItemResolver itemResolver)
    {
        foreach (var participant in GetParticipants())
            participant.Restore(gameData, itemResolver);
    }

    private IEnumerable<ISaveParticipant> GetParticipants()
    {
        var emitted = new HashSet<ISaveParticipant>();

        if (participantBehaviours != null)
        {
            foreach (var behaviour in participantBehaviours)
            {
                if (behaviour == null)
                    continue;

                if (behaviour is ISaveParticipant participant)
                {
                    if (emitted.Add(participant))
                        yield return participant;
                }
                else
                {
                    Debug.LogWarning($"[SaveParticipantRegistry] {behaviour.GetType().Name} no implementa ISaveParticipant y sera ignorado.", behaviour);
                }
            }
        }

        if (!includeParticipantsOnThisObject && !includeParticipantsInChildren)
            yield break;

        var behaviours = includeParticipantsInChildren
            ? GetComponentsInChildren<MonoBehaviour>(true)
            : GetComponents<MonoBehaviour>();

        foreach (var behaviour in behaviours)
        {
            if (behaviour == null || behaviour == this)
                continue;

            if (behaviour is ISaveParticipant participant && emitted.Add(participant))
                yield return participant;
        }
    }
}
