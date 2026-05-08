/*
 * Arquitectura: Scanner/Data
 * Script: ElementData
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona escaneo de elementos, datos escaneables y feedback visual del escaner.
 * Relaciones: Usa IScannable para escanear objetos sin conocer su implementacion concreta.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Element")]
public class ElementData : ScriptableObject
{
    public string elementName;
    public string symbol;
    public int rarity;
}