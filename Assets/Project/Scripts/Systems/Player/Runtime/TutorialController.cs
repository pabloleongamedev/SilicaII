/*
 * Arquitectura: Player/Runtime
 * Script: TutorialController
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actúa como facade o binding de escena.
 * Modulo: Gestiona estado global del jugador, input y bloqueos de gameplay/UI.
 * Relaciones: Coordina input, estados UI/gameplay y bloqueos globales usados por otros sistemas.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject controlesImage;

    [Header("GameState Controller")]
    [SerializeField] private GameStateController gameStateController;

    private void Awake()
    {
        if (gameStateController == null)
        {
            Debug.LogError("GameStateController no asignado en el Inspector.");
        }
    }

    private void Start()
    {
        StartCoroutine(StartTutorial());
    }

    private IEnumerator StartTutorial()
    {
        if (gameStateController == null)
            yield break;

        Debug.Log("INICIANDO TUTORIAL");

        // 🔥 BLOQUEAR
        gameStateController.SetState(GameState.Blocked);

        // 🔥 MOSTRAR
        controlesImage.SetActive(true);

        // 🔥 ESPERA
        yield return new WaitForSeconds(4f);

        // 🔥 OCULTAR
        controlesImage.SetActive(false);

        // 🔥 RESTAURAR
        gameStateController.SetState(GameState.Gameplay);

        Debug.Log("TUTORIAL TERMINADO");
    }
}
