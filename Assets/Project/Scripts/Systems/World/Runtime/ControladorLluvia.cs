/*
 * Arquitectura: World/Runtime
 * Script: ControladorLluvia
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona sistemas ambientales de escena como ciclo dia/noche y lluvia.
 * Relaciones: Opera en escena y puede ser consultado por sistemas visuales o ambientales.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class ControladorLluvia : MonoBehaviour
{
    [Header("Referencias")]
    public CicloDiaNoche scriptCiclo; // Arrastra aquí el objeto que tiene el script del sol
    public GameObject objetoLluvia;  // Arrastra aquí tu Particle System de lluvia

    [Header("Configuración")]
    [Range(0.0f, 23.0f)] public float horaInicioLluvia = 14f; 
    public float duracionLluviaEnMinutos = 1f; 

    void Update()
    {
        if (scriptCiclo == null || objetoLluvia == null) return;

        // Calculamos cuánto dura la lluvia en "horas de juego"
        // Como tu día dura 24 min reales, 1 min real = 1 hora de juego.
        float duracionEnHorasJuego = (duracionLluviaEnMinutos / scriptCiclo.DuracionDelDiaEnMinutos) * 24f;
        float horaFinLluvia = horaInicioLluvia + duracionEnHorasJuego;

        // Comprobamos la hora del script del ciclo
        float horaActual = scriptCiclo.hora;

        if (horaActual >= horaInicioLluvia && horaActual <= horaFinLluvia)
        {
            if (!objetoLluvia.activeSelf) objetoLluvia.SetActive(true);
        }
        else
        {
            if (objetoLluvia.activeSelf) objetoLluvia.SetActive(false);
        }
    }
}