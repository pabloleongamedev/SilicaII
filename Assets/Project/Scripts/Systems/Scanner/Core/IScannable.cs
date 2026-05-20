/*
 * Arquitectura: Scanner/Core
 * Script: IScannable
 * Rol: Contrato de dominio para objetos que pueden ser escaneados mediante el sistema de interaccion.
 * Modulo: Gestiona validacion de escaneo y exposicion de resultados detectables.
 * Relaciones: ItemPickup implementa este contrato cuando su InteractionMode es Scannable; ScannerTrigger solo lo recibe como contexto de feedback visual.
 * Uso como referencia: Scanner no conoce Inventory, Notification UI ni Player; solo expone datos escaneables como items runtime.
 */
using System.Collections.Generic;

public interface IScannable
{
    bool CanScan();
    IReadOnlyList<(ItemData_SO item, int amount)> GetScanOutputs();
    string GetScanInteractionText();
    void OnScanned();
}
