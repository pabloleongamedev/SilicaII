/*
 * Arquitectura: Menu/Runtime
 * Script: UnitySceneLoader
 * Rol: Fallback no-MonoBehaviour para codigo runtime que aun no tiene SceneLoadService asignado por Inspector.
 * Relaciones: Mantiene GameManager/Menu funcionales mientras migras escenas; la ruta recomendada sigue siendo SceneLoadService.
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitySceneLoader : ISceneLoader
{
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("[UnitySceneLoader] Nombre de escena vacio.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }
}
