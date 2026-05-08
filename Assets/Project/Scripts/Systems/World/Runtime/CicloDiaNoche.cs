/*
 * Arquitectura: World/Runtime
 * Script: CicloDiaNoche
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona sistemas ambientales de escena como ciclo dia/noche y lluvia.
 * Relaciones: Opera en escena y puede ser consultado por sistemas visuales o ambientales.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class CicloDiaNoche : MonoBehaviour
{
    [Range(0.0f, 24.0f)] public float hora = 9;
    public Transform sol; 
    public float DuracionDelDiaEnMinutos = 24f;

    private Light luzSol; // Para no usar GetComponent en cada frame
    private float solX;

    void Start()
    {
        if (sol != null)
        {
            luzSol = sol.GetComponent<Light>();
        }
    }

    private void Update()
    {
        // Agregamos 'f' a los números para que la división sea decimal
        // La fórmula es: (24h / (60min * Duración))
        hora += Time.deltaTime * (24f / (60f * DuracionDelDiaEnMinutos));

        if (hora >= 24)
        {
            hora = 0;
        }

        RotacionSol();
    }

    void RotacionSol()
    {
        solX = 15 * hora;
        
        if (sol != null) 
        {
            sol.localEulerAngles = new Vector3(solX, 0, 0);

            // Control de intensidad de luz
            if (luzSol != null)
            {
                if (hora < 6 || hora > 18)
                    luzSol.intensity = 0; 
                else
                    luzSol.intensity = 1; 
            }
        }
    }
}