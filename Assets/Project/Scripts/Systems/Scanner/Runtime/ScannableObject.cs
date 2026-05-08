using UnityEngine;

public class ScannableObject : MonoBehaviour, IScannable
{
    public ElementData data;

    public ElementData GetScanData()
    {
        return data;
    }

    public void OnScanned()
    {
        Debug.Log("Objeto escaneado");
        Destroy(gameObject);
    }
}