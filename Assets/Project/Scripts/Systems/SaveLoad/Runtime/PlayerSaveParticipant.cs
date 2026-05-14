/*
 * Arquitectura: SaveLoad/Runtime
 * Script: PlayerSaveParticipant
 * Rol: Participante de guardado para transform del jugador.
 * Relaciones: Implementa ISaveParticipant; lee/escribe PlayerSaveData sobre el transform del PlayerInputHandler asignado.
 * Riesgo arquitectonico mitigado: mueve persistencia del jugador fuera de GameManager y evita busquedas globales permanentes.
 */
using UnityEngine;

public class PlayerSaveParticipant : MonoBehaviour, ISaveParticipant
{
    [SerializeField] private PlayerInputHandler player;

    public void Capture(GameData gameData)
    {
        if (gameData == null || player == null)
            return;

        gameData.playerData.SetPosition(player.transform.position);
        gameData.playerData.SetRotation(player.transform.rotation);
    }

    public void Restore(GameData gameData, IItemResolver itemResolver)
    {
        if (gameData == null || player == null)
            return;

        player.transform.SetPositionAndRotation(
            gameData.playerData.GetPosition(),
            gameData.playerData.GetRotation()
        );
    }
}
