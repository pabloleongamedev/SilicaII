/*
 * Arquitectura: SaveLoad/Runtime
 * Script: SaveParticipantRegistry
 * Rol: Composition root local de participantes de guardado de una escena.
 * Relaciones: Recibe MonoBehaviours asignados por Inspector y ejecuta solo los que implementan ISaveParticipant.
 * Riesgo arquitectonico mitigado: reemplaza busquedas globales por registro explicito de dependencias de escena.
 */
using System.Collections.Generic;
using UnityEngine;

public class SaveParticipantRegistry : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] participantBehaviours;

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
        if (participantBehaviours == null)
            yield break;

        foreach (var behaviour in participantBehaviours)
        {
            if (behaviour is ISaveParticipant participant)
                yield return participant;
        }
    }
}
