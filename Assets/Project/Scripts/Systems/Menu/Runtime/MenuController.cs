/*
 * Arquitectura: Menu/Runtime
 * Script: MenuController
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Se conecta con SaveLoad/GameManager para iniciar, cargar y configurar partida.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // 🔥 SOLO ESTA FUNCIÓN
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Controllers (loading)
    }
}