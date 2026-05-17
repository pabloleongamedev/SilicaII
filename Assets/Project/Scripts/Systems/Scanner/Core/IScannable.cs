/*
 * Arquitectura: Scanner/Core
 * Script: IScannable
 * Rol: Contrato de dominio para objetos que pueden ser escaneados mediante el sistema de interaccion.
 * Modulo: Gestiona validacion de escaneo y exposicion de resultados detectables.
 * Relaciones: ScannableObject implementa este contrato; ScannerTrigger solo lo recibe como contexto de feedback visual.
 * Uso como referencia: Scanner no conoce Inventory, Notification UI ni Player; solo expone datos escaneables.
 */
public interface IScannable
{
    bool CanScan();
    ScanDefinition_SO GetScanDefinition();
    string GetScanInteractionText();
    void OnScanned();
}
