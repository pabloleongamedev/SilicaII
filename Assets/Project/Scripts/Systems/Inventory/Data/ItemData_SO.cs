/*
 * Arquitectura: Inventory/Data
 * Script: ItemData_SO
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/Item")]
public class ItemData_SO : ScriptableObject
{
    public string itemID;
    public string displayName;
    public Sprite icon;

    [Header("Stacking")]
    public int maxStack = 99;

    public int MaxStack => maxStack; // acceso consistente

    [TextArea]
    public string description;
}