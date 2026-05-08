public struct CraftingResult
{
    public bool success;
    public string message;
    public CraftingResultType type;

    public static CraftingResult Success(string msg)
    {
        return new CraftingResult
        {
            success = true,
            message = msg,
            type = CraftingResultType.Success
        };
    }

    public static CraftingResult Fail(string msg, CraftingResultType type = CraftingResultType.Warning)
    {
        return new CraftingResult
        {
            success = false,
            message = msg,
            type = type
        };
    }
}

public enum CraftingResultType
{
    Info,
    Success,
    Warning,
    Error
}
