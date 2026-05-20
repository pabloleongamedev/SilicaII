/*
 * Arquitectura: Interaction/Runtime
 * Script: ItemPickup
 * Rol: Interactable unico para objetos del mundo que contienen uno o mas items.
 * Modulo: Expone contenidos mediante IPickable y, segun InteractionMode, ejecuta recoleccion o escaneo.
 * Relaciones: InteractionDetector solo ve IInteractable; Inventory recibe items por InteractionContext; ScannerTrigger recibe feedback por ScannerEvents.
 * Riesgo arquitectonico mitigado: el modo explicito evita componentes redundantes cuando el diseno exige que un objeto sea pickable o scannable, nunca ambos.
 * Uso como referencia: un componente puede implementar varios contratos si el Inspector deja clara la decision de comportamiento.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable, IPickable, IScannable
{
    public enum InteractionMode
    {
        Pickable,
        Scannable
    }

    [Header("Interaction Mode")]
    [SerializeField] private InteractionMode interactionMode = InteractionMode.Pickable;

    [Header("Item Contents")]
    [SerializeField] private List<PickupItemEntry> items = new List<PickupItemEntry>();

    //[Header("Pickup Behaviour")]
    [SerializeField] private bool destroyAfterPickup = true;
    private string emptyPickupText = "Pickup Item";

    //[Header("Scan Behaviour")]
    [SerializeField] private bool allowRepeatScan = true;
    private string scanInteractionText = "Presiona E para escanear";
    private string scanningInteractionText = "Escaneando...";
    private string alreadyScannedText = "Analisis ya registrado";
    private string missingDataMessage = "No hay datos escaneables registrados.";

    [Header("Scan Notification")]
    [SerializeField] private string notificationTitle = "Analisis detectado";
    [SerializeField] private float notificationDelay = 2.1f;
    [SerializeField] private NotificationType successType = NotificationType.Info;
    [SerializeField] private NotificationType warningType = NotificationType.Warning;

    private bool isScanning;
    private bool wasScanned;

    public IReadOnlyList<PickupItemEntry> Items => items;

    [Serializable]
    public struct PickupItemEntry
    {
        [SerializeField] private ItemData_SO item;
        [SerializeField, Min(1)] private int amount;

        public ItemData_SO Item => item;
        public int Amount => Mathf.Max(1, amount);
        public bool IsValid => item != null && amount > 0;

        public PickupItemEntry(ItemData_SO item, int amount)
        {
            this.item = item;
            this.amount = Mathf.Max(1, amount);
        }
    }

    public void Interact(InteractionContext context)
    {
        if (interactionMode == InteractionMode.Scannable)
        {
            InteractAsScanner();
            return;
        }

        InteractAsPickup(context);
    }

    public string GetInteractionText()
    {
        return interactionMode == InteractionMode.Scannable
            ? GetScanInteractionText()
            : GetPickupInteractionText();
    }

    public IReadOnlyList<(ItemData_SO item, int amount)> GetPickableItems()
    {
        if (items == null || items.Count == 0)
            return Array.Empty<(ItemData_SO item, int amount)>();

        var validItems = new List<(ItemData_SO item, int amount)>();

        foreach (var entry in items)
        {
            if (!entry.IsValid)
                continue;

            validItems.Add((entry.Item, entry.Amount));
        }

        return validItems.ToArray();
    }

    public bool HasPickableItems()
    {
        return GetPickableItems().Count > 0;
    }

    public bool CanScan()
    {
        return interactionMode == InteractionMode.Scannable && HasPickableItems();
    }

    public IReadOnlyList<(ItemData_SO item, int amount)> GetScanOutputs()
    {
        return GetPickableItems();
    }

    public string GetScanInteractionText()
    {
        if (isScanning)
            return scanningInteractionText;

        if (wasScanned && !allowRepeatScan)
            return alreadyScannedText;

        return CanScan() ? scanInteractionText : missingDataMessage;
    }

    public void OnScanned()
    {
        wasScanned = true;

    }

    private void InteractAsPickup(InteractionContext context)
    {
        var validItems = ToArray(GetPickableItems());

        if (validItems.Length == 0)
            return;

        if (!context.InventoryRead.CanAddItemsBatch(validItems))
            return;

        foreach (var pickupItem in validItems)
            context.InventoryWrite.AddItem(pickupItem.item, pickupItem.amount);

        if (destroyAfterPickup)
            Destroy(gameObject);
    }

    private void InteractAsScanner()
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
        var outputs = GetScanOutputs();
        var builder = new StringBuilder();
        builder.Append(notificationTitle);
        builder.Append(": ");

        bool hasAnyOutput = false;

        foreach (var output in outputs)
        {
            if (output.item == null || output.amount <= 0)
                continue;

            if (hasAnyOutput)
                builder.Append(", ");

            builder.Append(GetItemDisplayName(output.item));
            //builder.Append(" x");
            //builder.Append(output.amount);
            hasAnyOutput = true;
        }

        return hasAnyOutput ? builder.ToString() : missingDataMessage;
    }

    private string GetPickupInteractionText()
    {
        if (!HasPickableItems())
            return emptyPickupText;

        return $"Presiona E para recoger {GetContentsLabel()}";
    }

    private string GetContentsLabel()
    {
        var validItems = GetPickableItems();

        if (validItems.Count == 0)
            return "objeto";

        if (validItems.Count == 1)
            return GetItemDisplayName(validItems[0].item);

        return $"{validItems.Count} recursos";
    }

    private string GetItemDisplayName(ItemData_SO itemData)
    {
        if (itemData == null)
            return "objeto";

        if (!string.IsNullOrEmpty(itemData.displayName))
            return itemData.displayName;

        return string.IsNullOrEmpty(itemData.itemID) ? itemData.name : itemData.itemID;
    }

    private void PublishNotification(string message, NotificationType type)
    {
        NotificationEvents.PublishNotification(new NotificationData
        {
            message = message,
            type = type
        });
    }

    private static (ItemData_SO item, int amount)[] ToArray(IReadOnlyList<(ItemData_SO item, int amount)> source)
    {
        if (source == null || source.Count == 0)
            return Array.Empty<(ItemData_SO item, int amount)>();

        var result = new (ItemData_SO item, int amount)[source.Count];

        for (int i = 0; i < source.Count; i++)
            result[i] = source[i];

        return result;
    }
}
