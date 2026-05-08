using UnityEngine;

public class ScannerController : MonoBehaviour
{
    private ScanSystem scanSystem;

    private void Awake()
    {
        scanSystem = new ScanSystem();
    }

    public void TryScan()
    {
        Ray ray = Camera.main.ViewportPointToRay(Vector3.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            var scannable = hit.collider.GetComponent<IScannable>();

            if (scannable != null)
            {
                scanSystem.Scan(scannable);
            }
        }
    }

    public ScanSystem GetSystem()
    {
        return scanSystem;
    }
}