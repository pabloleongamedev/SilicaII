using System;

public class ScanSystem
{
    public Action<ElementData> OnScanCompleted;

    public void Scan(IScannable target)
    {
        var data = target.GetScanData();

        target.OnScanned();

        OnScanCompleted?.Invoke(data);
    }
}