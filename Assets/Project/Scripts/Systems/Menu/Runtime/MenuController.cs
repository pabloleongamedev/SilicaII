/*
 * Arquitectura: Menu/Runtime
 * Script: MenuController
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Usa ISceneLoader para entrar al flujo de carga sin depender de SceneManager directo.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MonoBehaviour sceneLoaderBehaviour;
    [SerializeField] private int loadingSceneIndex = 1;

    private ISceneLoader sceneLoader;

    private void Awake()
    {
        sceneLoader = ResolveSceneLoader();
    }

    public void StartGame()
    {
        ResolveSceneLoader().LoadScene(loadingSceneIndex);
    }

    private ISceneLoader ResolveSceneLoader()
    {
        if (sceneLoader == null)
            sceneLoader = sceneLoaderBehaviour as ISceneLoader;

        if (sceneLoader == null && sceneLoaderBehaviour != null)
            Debug.LogWarning("[MenuController] El Scene Loader asignado no implementa ISceneLoader.", this);

        if (sceneLoader == null)
            sceneLoader = new UnitySceneLoader();

        return sceneLoader;
    }
}
