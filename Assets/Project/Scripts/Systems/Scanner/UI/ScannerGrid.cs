/*
 * Arquitectura: Scanner/UI
 * Script: ScannerGrid
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona escaneo de elementos, datos escaneables y feedback visual del escaner.
 * Relaciones: Usa IScannable para escanear objetos sin conocer su implementacion concreta.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using TMPro;

public class ScannerGrid : MonoBehaviour
{
    [Header("UI del Escaner")]
    public GameObject canvasElementName;
    public TextMeshProUGUI elementNameText;
    public float height = 1.5f; // Duración para mostrar el nombre del elemento
    private void OnTriggerEnter(Collider other)
    {
        // Si la malla toca algo con el Tag Element
        if (other.CompareTag("Element"))
        {
            elementNameText.text = other.gameObject.name;
            Vector3 positionNameElement = other.bounds.center;
            canvasElementName.transform.position = positionNameElement + Vector3.up * height; // Ajusta la altura del Canvas    
            // CAPTURA DIRECTA: Imprime el nombre del objeto que tocó
            Debug.Log("Malla detectó: " + other.gameObject.name);
            canvasElementName.SetActive(true);
            // Aquí es donde luego pondremos la línea para el Canvas
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Element"))
        {
            canvasElementName.SetActive(false);
        }
    }
    private void LateUpdate()
    {
        if (canvasElementName.activeSelf)
        {
            // Asegura que el Canvas siempre mire hacia la cámara
            canvasElementName.transform.LookAt(canvasElementName.transform.position + Camera.main.transform.forward);

        }
    }
}
