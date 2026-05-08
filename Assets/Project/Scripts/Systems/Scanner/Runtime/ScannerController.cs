/*
 * Arquitectura: Scanner/Runtime
 * Script: ScannerController
 * Rol: Conecta Unity con el Core. Lee componentes, recibe input/eventos y actua como facade o binding de escena.
 * Modulo: Gestiona escaneo de elementos, datos escaneables y feedback visual del escaner.
 * Relaciones: Usa IScannable para escanear objetos sin conocer su implementacion concreta.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

public class ScannerController : MonoBehaviour
{
    private ScanSystem scanSystem;

    private void Awake()
    {
        scanSystem = new ScanSystem();
    }

    public void TryScan()
    {
        Ray ray = Camera.main.ViewportPointToRay(Vector3.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            var scannable = hit.collider.GetComponent<IScannable>();

            if (scannable != null)
            {
                scanSystem.Scan(scannable);
            }
        }
    }

    public ScanSystem GetSystem()
    {
        return scanSystem;
    }
}