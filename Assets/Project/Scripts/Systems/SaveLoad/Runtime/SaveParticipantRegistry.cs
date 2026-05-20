/*
 * Arquitectura: SaveLoad/Runtime
 * Script: SaveParticipantRegistry
 * Rol: Composition root local de participantes de guardado de una escena.
 * Relaciones: Recibe MonoBehaviours asignados por Inspector y tambien descubre participantes en el mismo GameObject/hijos.
 * Riesgo arquitectonico mitigado: reemplaza busquedas globales; SaveLoadSceneBinding lo usa como ruta oficial de participantes.
 */
using System.Collections.Generic;
using UnityEngine;

public class SaveParticipantRegistry : MonoBehaviour
{
    [SerializeField] private ItemDatabase_SO itemDatabase;
    [SerializeField] private MonoBehaviour[] participantBehaviours;
    [SerializeField] private bool includeParticipantsOnThisObject = true;
    [SerializeField] private bool includeParticipantsInChildren = false;

    public ItemDatabase_SO ItemDatabase => itemDatabase;

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
