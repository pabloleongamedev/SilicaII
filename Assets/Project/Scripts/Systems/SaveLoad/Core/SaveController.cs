/*
 * Arquitectura: SaveLoad/Core
 * Script: SaveController
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona datos de partida, guardado/carga y restauracion de estado runtime.
 * Relaciones: Consulta facades runtime como InventoryController y PlayerInputHandler para persistir/restaurar datos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using System.IO;

/// <summary>
/// Controlador para guardar y cargar datos del juego en disco.
/// Maneja la serialización JSON de GameData.
/// </summary>
public class SaveController
{
    private string saveFolderPath;

    public SaveController()
    {
        // Usar persistentDataPath para compatibilidad multiplataforma
        saveFolderPath = Path.Combine(Application.persistentDataPath, "Saves");

        // Crear carpeta si no existe
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
            Debug.Log($"[SaveController] Carpeta de guardado creada en: {saveFolderPath}");
        }
    }

    /// <summary>
    /// Obtiene la ruta completa del archivo de guardado para un slot específico
    /// </summary>
    public string GetSaveFilePath(string slotID)
    {
        return Path.Combine(saveFolderPath, $"save_{slotID}.json");
    }

    /// <summary>
    /// Verifica si existe un archivo de guardado para un slot específico
    /// </summary>
    public bool HasSaveFile(string slotID)
    {
        string filePath = GetSaveFilePath(slotID);
        return File.Exists(filePath);
    }

    /// <summary>
    /// Guarda GameData en un archivo JSON
    /// </summary>
    public void SaveGame(GameData gameData, string slotID)
    {
        gameData.slotID = slotID;
        gameData.lastSaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string filePath = GetSaveFilePath(slotID);

        try
        {
            // Serializar a JSON
            string json = JsonUtility.ToJson(gameData, true);

            // Escribir archivo
            File.WriteAllText(filePath, json);
            Debug.Log($"[SaveController] Partida guardada en: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveController] Error al guardar partida: {e.Message}");
        }
    }

    /// <summary>
    /// Carga GameData desde un archivo JSON
    /// </summary>
    public GameData LoadGame(string slotID)
    {
        string filePath = GetSaveFilePath(slotID);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"[SaveController] No se encontró archivo de guardado: {filePath}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log($"[SaveController] Partida cargada desde: {filePath}");
            return gameData;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveController] Error al cargar partida: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Elimina un archivo de guardado (para borrar partidas)
    /// </summary>
    public void DeleteSave(string slotID)
    {
        string filePath = GetSaveFilePath(slotID);

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log($"[SaveController] Partida eliminada: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveController] Error al eliminar partida: {e.Message}");
            }
        }
    }

    /// <summary>
    /// Obtiene información de un guardado sin cargar toda la partida
    /// (útil para mostrar en el menú)
    /// </summary>
    public SaveInfo GetSaveInfo(string slotID)
    {
        if (!HasSaveFile(slotID))
            return null;

        GameData data = LoadGame(slotID);
        if (data == null)
            return null;

        return new SaveInfo
        {
            slotID = slotID,
            scene = data.currentScene,
            lastSaveTime = data.lastSaveTime,
            playTime = data.GetPlayTimeFormatted(),
            playTimeSeconds = data.playTimeSeconds
        };
    }

    /// <summary>
    /// Obtiene información de todos los guardos disponibles
    /// </summary>
    public SaveInfo[] GetAllSaveInfos()
    {
        SaveInfo[] infos = new SaveInfo[3];

        for (int i = 1; i <= 3; i++)
        {
            infos[i - 1] = GetSaveInfo(i.ToString());
        }

        return infos;
    }
}

/// <summary>
/// Información condensada de un guardado (para UI)
/// </summary>
[System.Serializable]
public class SaveInfo
{
    public string slotID;
    public string scene;
    public string lastSaveTime;
    public string playTime;
    public int playTimeSeconds;
}
