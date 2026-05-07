public struct CraftingResult
{
    public bool success;
    public string message;
    public NotificationType type;

    public static CraftingResult Success(string msg)
    {
        return new CraftingResult
        {
            success = true,
            message = msg,
            type = NotificationType.Success
        };
    }

    public static CraftingResult Fail(string msg, NotificationType type = NotificationType.Warning)
    {
        return new CraftingResult
        {
            success = false,
            message = msg,
            type = type
        };
    }
}