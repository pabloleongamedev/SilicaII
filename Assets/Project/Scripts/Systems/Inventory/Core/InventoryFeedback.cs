public struct InventoryFeedback
{
    public string message;
    public InventoryFeedbackType type;
}

public enum InventoryFeedbackType
{
    Info,
    Success,
    Warning,
    Error
}
