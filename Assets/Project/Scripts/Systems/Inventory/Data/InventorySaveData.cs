/// <summary>
/// Datos serializables de un item en el inventario.
/// </summary>
[System.Serializable]
public class InventorySaveData
{
    public string itemID;
    public int gridX;
    public int gridY;
    public int quantity = 1;
}
