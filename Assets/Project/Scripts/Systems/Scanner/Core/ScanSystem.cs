/*
 * Arquitectura: Scanner/Core
 * Script: ScanSystem
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona escaneo de elementos, datos escaneables y feedback visual del escaner.
 * Relaciones: Usa IScannable para escanear objetos sin conocer su implementacion concreta.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;

public class ScanSystem
{
    public Action<ElementData> OnScanCompleted;

    public void Scan(IScannable target)
    {
        var data = target.GetScanData();

        target.OnScanned();

        OnScanCompleted?.Invoke(data);
    }
}