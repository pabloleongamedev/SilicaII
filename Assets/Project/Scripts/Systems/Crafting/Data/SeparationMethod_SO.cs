/*
 * Arquitectura: Crafting/Data
 * Script: SeparationMethod_SO
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

[CreateAssetMenu(menuName = "Chemistry/Separation Method")]
public class SeparationMethod_SO : ScriptableObject
{
    public string methodName;
    public string description;
}