/*
 * Arquitectura: Scanner/Runtime
 * Script: ScannableObject
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona escaneo de elementos, datos escaneables y feedback visual del escaner.
 * Relaciones: Usa IScannable para escanear objetos sin conocer su implementacion concreta.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class ScannableObject : MonoBehaviour, IScannable
{
    public ElementData data;

    public ElementData GetScanData()
    {
        return data;
    }

    public void OnScanned()
    {
        Debug.Log("Objeto escaneado");
        Destroy(gameObject);
    }
}