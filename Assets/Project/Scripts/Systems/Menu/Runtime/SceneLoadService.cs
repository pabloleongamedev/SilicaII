/*
 * Arquitectura: Menu/Runtime
 * Script: SceneLoadService
 * Rol: Adapter Unity para cargar escenas.
 * Relaciones: Implementa ISceneLoader; UI y SaveLoad pueden recibirlo por Inspector o usar UnitySceneLoader como fallback temporal.
 * Uso como referencia: si luego agregas loading screen, fade, async loading o SceneCatalog_SO, el cambio vive aqui.
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadService : MonoBehaviour, ISceneLoader
{
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("[SceneLoadService] Nombre de escena vacio.", this);
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }
}
