/*
 * Arquitectura: SaveLoad/Runtime
 * Script: SaveParticipantRegistry
 * Rol: Composition root local de participantes de guardado de una escena.
 * Relaciones: Recibe MonoBehaviours asignados por Inspector como ruta principal.
 * Riesgo arquitectonico mitigado: reemplaza busquedas globales; SaveLoadSceneBinding lo usa como ruta oficial de participantes.
 */
using System.Collections.Generic;
using UnityEngine;

public class SaveParticipantRegistry : MonoBehaviour
{
    private const string MigrationDiscoveryWarning =
        "[SaveParticipantRegistry] No hay participantes explicitos en participantBehaviours. " +
        "Se usara auto-descubrimiento local solo como ayuda temporal de migracion; configura las referencias por Inspector.";

    [SerializeField] private ItemDatabase_SO itemDatabase;

    [Header("Explicit Participants")]
    [SerializeField] private MonoBehaviour[] participantBehaviours;

    [Header("Migration Discovery")]
    [SerializeField] private bool includeParticipantsOnThisObject = false;
    [SerializeField] private bool includeParticipantsInChildren = false;

    private bool warnedMigrationDiscovery;

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
        bool hasExplicitParticipant = false;

        if (participantBehaviours != null)
        {
            foreach (var behaviour in participantBehaviours)
            {
                if (behaviour == null)
                    continue;

                if (behaviour is ISaveParticipant participant)
                {
                    hasExplicitParticipant = true;

                    if (emitted.Add(participant))
                        yield return participant;
                }
                else
                {
                    Debug.LogWarning($"[SaveParticipantRegistry] {behaviour.GetType().Name} no implementa ISaveParticipant y sera ignorado.", behaviour);
                }
            }
        }

        if (hasExplicitParticipant)
            yield break;

        if (!includeParticipantsOnThisObject && !includeParticipantsInChildren)
        {
            Debug.LogWarning("[SaveParticipantRegistry] participantBehaviours no tiene ISaveParticipant asignados. No se capturara/restaurara estado runtime.", this);
            yield break;
        }

        WarnMigrationDiscovery();

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

    private void WarnMigrationDiscovery()
    {
        if (warnedMigrationDiscovery)
            return;

        warnedMigrationDiscovery = true;
        Debug.LogWarning(MigrationDiscoveryWarning, this);
    }
}
