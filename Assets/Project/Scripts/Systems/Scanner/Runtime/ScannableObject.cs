/*
 * Arquitectura: Scanner/Runtime
 * Script: ScannableObject
 * Rol: Interactable de mundo para objetos escaneables.
 * Modulo: Usa Interaction como entrada, valida IScannable, solicita feedback al ScannerTrigger y publica NotificationData con outputs.
 * Data: usa ScanDefinition_SO propio del sistema Scanner; no depende de CompoundDefinition_SO de Crafting.
 * Relaciones: InteractionDetector lo detecta como IInteractable; ScannerEvents activa animacion/audio; NotificationEvents muestra resultados.
 * Uso como referencia: el objeto escaneable no conoce PlayerInputHandler, ScannerTrigger, NotificationView ni AudioManager.
 */
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ScannableObject : MonoBehaviour, IInteractable, IScannable
{
    [Header("Scan Data")]
    [SerializeField] private ScanDefinition_SO scanDefinition;
    [SerializeField] private bool destroyAfterScan;
    [SerializeField] private bool allowRepeatScan = true;

    [Header("Interaction Messages")]
    [SerializeField] private string scanInteractionText = "Presiona E para escanear";
    [SerializeField] private string scanningInteractionText = "Escaneando...";
    [SerializeField] private string alreadyScannedText = "Analisis ya registrado";
    [SerializeField] private string missingDataMessage = "No hay datos escaneables registrados.";

    [Header("Notification")]
    [SerializeField] private string notificationTitle = "Analisis detectado";
    [SerializeField] private float notificationDelay = 0.5f;
    [SerializeField] private NotificationType successType = NotificationType.Info;
    [SerializeField] private NotificationType warningType = NotificationType.Warning;

    private bool isScanning;
    private bool wasScanned;

    public void Interact(InteractionContext context)
    {
        if (isScanning)
            return;

        if (!CanScan())
        {
            PublishNotification(missingDataMessage, warningType);
            return;
        }

        if (wasScanned && !allowRepeatScan)
        {
            PublishNotification(alreadyScannedText, warningType);
            return;
        }

        StartCoroutine(ScanRoutine());
    }

    public string GetInteractionText()
    {
        if (isScanning)
            return scanningInteractionText;

        if (wasScanned && !allowRepeatScan)
            return alreadyScannedText;

        return CanScan() ? scanInteractionText : missingDataMessage;
    }

    public bool CanScan()
    {
        return scanDefinition != null && HasValidOutputs(scanDefinition.Outputs);
    }

    public ScanDefinition_SO GetScanDefinition()
    {
        return scanDefinition;
    }

    public string GetScanInteractionText()
    {
        return GetInteractionText();
    }

    public void OnScanned()
    {
        wasScanned = true;

        if (destroyAfterScan)
            Destroy(gameObject);
    }

    private IEnumerator ScanRoutine()
    {
        isScanning = true;
        ScannerEvents.RequestScanFeedback(this);

        if (notificationDelay > 0f)
            yield return new WaitForSeconds(notificationDelay);

        PublishNotification(BuildOutputsMessage(), successType);
        OnScanned();
        isScanning = false;
    }

    private string BuildOutputsMessage()
    {
        var builder = new StringBuilder();
        builder.Append(notificationTitle);
        builder.Append(": ");

        bool hasAnyOutput = false;

        foreach (var output in scanDefinition.Outputs)
        {
            if (output.item == null || output.amount <= 0)
                continue;

            if (hasAnyOutput)
                builder.Append(", ");

            builder.Append(GetItemLabel(output.item));
            builder.Append(" x");
            builder.Append(output.amount);
            hasAnyOutput = true;
        }

        if (!hasAnyOutput)
            return missingDataMessage;

        return builder.ToString();
    }

    private string GetItemLabel(ItemData_SO item)
    {
        if (item == null)
            return "Desconocido";

        if (!string.IsNullOrEmpty(item.displayName))
            return item.displayName;

        return string.IsNullOrEmpty(item.itemID) ? item.name : item.itemID;
    }

    private bool HasValidOutputs(IReadOnlyList<ScanDefinition_SO.ScanOutput> outputs)
    {
        if (outputs == null || outputs.Count == 0)
            return false;

        foreach (var output in outputs)
        {
            if (output.item != null && output.amount > 0)
                return true;
        }

        return false;
    }

    private void PublishNotification(string message, NotificationType type)
    {
        NotificationEvents.OnNotification?.Invoke(new NotificationData
        {
            message = message,
            type = type
        });
    }
}
