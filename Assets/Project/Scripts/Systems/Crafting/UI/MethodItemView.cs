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