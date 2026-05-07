using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject controlesImage;

    private GameStateController gameStateController;


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