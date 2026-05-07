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