/*
 * Arquitectura: Interaction/Core
 * Script: IPickable
 * Rol: Contrato de datos para objetos del mundo que contienen items recolectables o escaneables.
 * Relaciones: ItemPickup implementa este contrato y decide por InteractionMode si los contenidos se recogen o se escanean.
 * Uso como referencia: separar datos del mundo de la accion evita que InteractionDetector tenga reglas especiales por sistema.
 */
using System.Collections.Generic;

public interface IPickable
{
    IReadOnlyList<(ItemData_SO item, int amount)> GetPickableItems();
    bool HasPickableItems();
}
