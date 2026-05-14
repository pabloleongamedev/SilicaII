/*
 * Arquitectura: Menu/UI
 * Script: SaveSlot
 * Rol: Vista/controlador de un slot de guardado simplificado.
 * Modulo: Gestiona pantallas, configuraciones y flujo del menu.
 * Relaciones: Lee SaveInfo desde GameManager.Instance y dispara LoadGame/CreateNewGame sobre el slot unico "1".
 * Riesgo arquitectonico: mezcla presentacion con flujo de SaveLoad; debe depender de ISaveSlotService o emitir evento hacia MainMenuFlowController.
 * Uso como referencia: muestra el contrato visual esperado del slot, pero la decision de cargar/crear debe salir de la vista.
 */
using UnityEngine;
using TMPro;

/// <summary>
/// SaveSlot: Botón para continuar o crear nueva partida.
/// Sistema simplificado de una única partida guardada.
/// 
/// FUNCIONALIDAD:
/// - Botón "Continuar" si hay partida guardada
/// - Botón "Nueva Partida" si no hay guardado
/// - Usa siempre slot "1" (una única partida)
/// </summary>
public class SaveSlot : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private TextMeshProUGUI textLabel;
    [SerializeField] private TextMeshProUGUI infoLabel; // Opcional: muestra tiempo de juego

    // Siempre usar slot "1" (partida única)
    private const string UNIQUE_SLOT = "1";
    private bool hasData = false;
    private SaveInfo saveInfo;

    void Start()
    {
        // Asegurarse de que GameManager esté inicializado
        if (GameManager.Instance == null)
        {
            Debug.LogError("[SaveSlot] GameManager no encontrado en la escena");
            enabled = false;
            return;
        }

        //RefreshSlot();
    }

    /// <summary>
    /// Refresca el estado consultando GameManager
    /// </summary>
    public void RefreshSlot()
    {
        if (GameManager.Instance == null)
            return;

        hasData = GameManager.Instance.HasSaveFile(UNIQUE_SLOT);
        saveInfo = GameManager.Instance.GetSaveInfo(UNIQUE_SLOT);
        UpdateSlotVisual();
    }

    /// <summary>
    /// Actualiza la UI del botón según si hay guardado o no
    /// </summary>
    private void UpdateSlotVisual()
    {
        if (textLabel == null)
        {
            Debug.LogError("[SaveSlot] TextLabel no asignado", this);
            return;
        }

        if (hasData && saveInfo != null)
        {
            // Hay guardado: mostrar "Continuar"
            textLabel.text = "Continuar";
            
            if (infoLabel != null)
            {
                infoLabel.text = $"{saveInfo.playTime} | {saveInfo.lastSaveTime}";
            }
        }
        else
        {
            // Sin guardado: mostrar "Nueva Partida"
            textLabel.text = "Nueva Partida";
            
            if (infoLabel != null)
            {
                infoLabel.text = "Sin partida guardada";
            }
        }
    }

    /// <summary>
    /// Se llama cuando el usuario presiona el botón
    /// </summary>
    public void OnSlotPressed()
    {
        if (hasData)
        {
            Debug.Log($"[SaveSlot] Cargando partida guardada...");
            GameManager.Instance.LoadGame(UNIQUE_SLOT);
        }
        else
        {
            Debug.Log($"[SaveSlot] Creando nueva partida...");
            GameManager.Instance.CreateNewGame(UNIQUE_SLOT);
        }
    }
}
