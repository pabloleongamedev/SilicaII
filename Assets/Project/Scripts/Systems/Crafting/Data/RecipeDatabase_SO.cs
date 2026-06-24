/*
 * Arquitectura: Crafting/Data
 * Script: RecipeDatabase_SO
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe Database")]
public class RecipeDatabase_SO : ScriptableObject
{
    public List<RecipeData_SO> recipes;
}