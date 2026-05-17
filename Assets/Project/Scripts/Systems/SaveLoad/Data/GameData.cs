/*
 * Arquitectura: SaveLoad/Data
 * Script: GameData
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona datos de partida, guardado/carga y restauracion de estado runtime.
 * Relaciones: Consulta facades runtime como InventoryController y PlayerInputHandler para persistir/restaurar datos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase serializable que contiene todos los datos de una partida guardada.
/// Se utiliza JsonUtility para serializar/deserializar a JSON.
/// </summary>
[System.Serializable]
public class GameData
{
    [Header("Información de la Partida")]
    public string slotID = "1";
    public string lastSaveTime = "";
    public int playTimeSeconds = 0;

    [Header("Datos del Jugador")]
    public PlayerSaveData playerData = new PlayerSaveData();

    [Header("Inventario")]
    public List<InventorySaveData> inventoryItems = new List<InventorySaveData>();

    [Header("Progreso")]
    public string currentScene = "MainMenu";
    public float missionTimeRemaining = -1f;
    public float missionDuration = -1f;
    public List<string> scannedElements = new List<string>();

    [Header("Estado del Mundo")]
    public List<string> collectedItems = new List<string>();
    public List<string> destroyedObjects = new List<string>();

    /// <summary>
    /// Crea una copia limpia para una nueva partida
    /// </summary>
    public static GameData CreateNewGame(string slotID)
    {
        return new GameData
        {
            slotID = slotID,
            lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            playTimeSeconds = 0,
            playerData = PlayerSaveData.CreateDefault(),
            currentScene = "Pablo_TestMechanics",
            missionTimeRemaining = -1f,
            missionDuration = -1f,
            inventoryItems = new List<InventorySaveData>(),
            scannedElements = new List<string>(),
            collectedItems = new List<string>(),
            destroyedObjects = new List<string>()
        };
    }

    /// <summary>
    /// Actualiza el tiempo de juego transcurrido
    /// </summary>
    public void UpdatePlayTime(int deltaSeconds)
    {
        playTimeSeconds += deltaSeconds;
        lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// Retorna una representación legible del tiempo jugado
    /// </summary>
    public string GetPlayTimeFormatted()
    {
        int hours = playTimeSeconds / 3600;
        int minutes = (playTimeSeconds % 3600) / 60;
        return $"{hours}h {minutes}m";
    }
}

/// <summary>
/// Datos del jugador: posición, rotación, salud, etc.
/// </summary>
[System.Serializable]
public class PlayerSaveData
{
    public float posX = 0;
    public float posY = 1;
    public float posZ = 0;

    public float rotX = 0;
    public float rotY = 0;
    public float rotZ = 0;
    public float rotW = 1;

    public int health = 100;
    public int maxHealth = 100;
    public float jetpackFuel = 100;

    public static PlayerSaveData CreateDefault()
    {
        return new PlayerSaveData
        {
            posX = 0,
            posY = 1,
            posZ = 0,
            health = 100,
            maxHealth = 100,
            jetpackFuel = 100
        };
    }

    /// <summary>
    /// Convierte a Vector3 la posición guardada
    /// </summary>
    public Vector3 GetPosition()
    {
        return new Vector3(posX, posY, posZ);
    }

    /// <summary>
    /// Convierte a Quaternion la rotación guardada
    /// </summary>
    public Quaternion GetRotation()
    {
        return new Quaternion(rotX, rotY, rotZ, rotW);
    }

    /// <summary>
    /// Guarda una posición Vector3
    /// </summary>
    public void SetPosition(Vector3 pos)
    {
        posX = pos.x;
        posY = pos.y;
        posZ = pos.z;
    }

    /// <summary>
    /// Guarda una rotación Quaternion
    /// </summary>
    public void SetRotation(Quaternion rot)
    {
        rotX = rot.x;
        rotY = rot.y;
        rotZ = rot.z;
        rotW = rot.w;
    }
}
