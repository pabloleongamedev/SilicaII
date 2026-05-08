/*
 * Arquitectura: Crafting/UI
 * Script: MethodItemView
 * Rol: Presenta informacion y captura intenciones de usuario. Debe delegar reglas de gameplay a Runtime/Core.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class MethodItemView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI nameText;

    private SeparationMethod_SO method;
    private Action<SeparationMethod_SO> onClick;

    public void Setup(SeparationMethod_SO method, Action<SeparationMethod_SO> onClick)
    {
        this.method = method;
        this.onClick = onClick;

        nameText.text = method.methodName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(method);
    }
}