/*
 * Arquitectura: SaveLoad/Debug
 * Script: GameDebugger
 * Rol: Apoyo para depuracion, documentacion o pruebas manuales. No debe ser dependencia de gameplay de produccion.
 * Modulo: Gestiona datos de partida, guardado/carga y restauracion de estado runtime.
 * Relaciones: Consulta facades runtime como InventoryController y PlayerInputHandler para persistir/restaurar datos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// GameDebugger: Utilidad para debugging del sistema de guardado en Editor.
/// 
/// ATAJO EN EDITOR:
/// 1. Window > GameDebugger
/// 2. Ver estado actual del juego
/// 3. Simular save/load
/// 4. Ver archivos guardados
/// 5. Limpiar saves
/// 
/// También puedes llamar métodos directamente desde Console:
/// GameDebugger.PrintGameState();
/// GameDebugger.DeleteAllSaves();
/// </summary>
public class GameDebugger : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        // Se ejecuta automáticamente al entrar en Play Mode
    }

    /// <summary>
    /// Imprime el estado actual del GameManager en la consola
    /// </summary>
    public static void PrintGameState()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no inicializado");
            return;
        }

        //Debug.Log(GameManager.Instance.DebugGetGameState());
    }

    /// <summary>
    /// Imprime información de todos los saves
    /// </summary>
  /*  public static void PrintAllSaves()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no inicializado");
            return;
        }

        SaveInfo[] infos = GameManager.Instance.GetAllSaveInfos();

        string output = "\n=== INFORMACIÓN DE TODOS LOS SAVES ===\n";

        for (int i = 0; i < infos.Length; i++)
        {
            if (infos[i] != null)
            {
                output += $"Slot {i + 1}: {infos[i].scene} - {infos[i].playTime} h\n";
            }
            else
            {
                output += $"Slot {i + 1}: (vacío)\n";
            }
        }

        output += "====================================\n";
        Debug.Log(output);
    }

    /// <summary>
    /// Simula un auto-save manual (para testing)
    /// </summary>
    public static void ForceAutoSave()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no inicializado");
            return;
        }

        GameManager.Instance.SaveGame();
        Debug.Log("AUTO-SAVE forzado completado");
    }

    /// <summary>
    /// Elimina todos los archivos de guardado (CUIDADO)
    /// </summary>
    public static void DeleteAllSaves()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager no inicializado");
            return;
        }

        SaveController controller = new SaveController();

        for (int i = 1; i <= 3; i++)
        {
            controller.DeleteSave(i.ToString());
        }

        Debug.Log("TODOS LOS SAVES ELIMINADOS");
    }

    /// <summary>
    /// Obtiene la ruta de la carpeta de guardos
    /// </summary>
    public static void PrintSavePath()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "Saves");
        Debug.Log($"Ruta de guardos: {path}");
    }

    /// <summary>
    /// Abre el explorador en la carpeta de guardos (solo en Windows)
    /// </summary>
    public static void OpenSaveFolder()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "Saves");

#if UNITY_EDITOR_WIN
        System.Diagnostics.Process.Start("explorer.exe", path);
        Debug.Log($"Abriendo carpeta: {path}");
#elif UNITY_EDITOR_OSX
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
        psi.FileName = "open";
        psi.Arguments = path;
        System.Diagnostics.Process.Start(psi);
        Debug.Log($"Abriendo carpeta: {path}");
#else
        Debug.Log($"Carpeta de guardos: {path}");
#endif
    }
}

#if UNITY_EDITOR

/// <summary>
/// Editor Window para debugging del sistema de guardado
/// </summary>
public class GameDebuggerWindow : EditorWindow
{
    [MenuItem("Window/GameDebugger")]
    public static void ShowWindow()
    {
        GetWindow<GameDebuggerWindow>("GameDebugger");
    }

    private void OnGUI()
    {
        GUILayout.Label("🎮 Game Save & Load Debugger", EditorStyles.boldLabel);

        GUILayout.Space(10);

        // Estado actual
        if (GUILayout.Button("📊 Mostrar Estado del Juego", GUILayout.Height(30)))
        {
            GameDebugger.PrintGameState();
        }

        if (GUILayout.Button("📋 Mostrar Información de Saves", GUILayout.Height(30)))
        {
            GameDebugger.PrintAllSaves();
        }

        GUILayout.Space(10);

        // Testing
        GUILayout.Label("🧪 Testing", EditorStyles.boldLabel);

        if (GUILayout.Button("💾 Forzar Auto-Save", GUILayout.Height(30)))
        {
            GameDebugger.ForceAutoSave();
        }

        GUILayout.Space(10);

        // Utilidades
        GUILayout.Label("⚙️ Utilidades", EditorStyles.boldLabel);

        if (GUILayout.Button("📁 Mostrar Ruta de Guardos", GUILayout.Height(30)))
        {
            GameDebugger.PrintSavePath();
        }

        if (GUILayout.Button("📂 Abrir Carpeta de Guardos", GUILayout.Height(30)))
        {
            GameDebugger.OpenSaveFolder();
        }

        GUILayout.Space(20);

        // Peligroso
        GUILayout.Label("⚠️ Operaciones Destructivas", EditorStyles.boldLabel);

        GUI.color = Color.red;
        if (GUILayout.Button("🗑️ ELIMINAR TODOS LOS SAVES", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog(
                "Confirmación",
                "¿Estás seguro de que quieres ELIMINAR TODOS LOS SAVES?",
                "SÍ, eliminar",
                "Cancelar"))
            {
                GameDebugger.DeleteAllSaves();
            }
        }
        GUI.color = Color.white;

        GUILayout.Space(20);

        GUILayout.Label("💡 Tips", EditorStyles.boldLabel);
        GUILayout.TextArea(
            "• Los datos se guardan en formato JSON\n" +
            "• Se pueden editar manualmente\n" +
            "• Auto-save cada 60 segundos\n" +
            "• Presiona Jugar → ver saves detectados\n" +
            "• GameManager persiste entre escenas",
            GUILayout.Height(100));
    }
}

#endif
*/}