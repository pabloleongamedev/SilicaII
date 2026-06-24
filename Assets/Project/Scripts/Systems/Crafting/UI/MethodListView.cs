/*
 * Arquitectura: Crafting/UI
 * Script: MethodListView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using System;

public class MethodListView : MonoBehaviour
{
    [SerializeField] private MethodItemView prefab;
    [SerializeField] private Transform container;

    public void Build(SeparationMethod_SO[] methods, Action<SeparationMethod_SO> onClick)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach (var method in methods)
        {
            var item = Instantiate(prefab, container);
            item.Setup(method, onClick);
        }
    }
}