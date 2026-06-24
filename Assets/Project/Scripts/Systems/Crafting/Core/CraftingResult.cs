/*
 * Arquitectura: Crafting/Core
 * Script: CraftingResult
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona recetas, crafting y separacion quimica; consume/produce items mediante los contratos de Inventory.
 * Relaciones: Se relaciona con Inventory para consumir/producir items y con Quest/Notification mediante eventos de Runtime.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
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
