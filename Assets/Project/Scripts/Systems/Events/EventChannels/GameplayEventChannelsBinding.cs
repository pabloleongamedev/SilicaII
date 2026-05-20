/*
 * Arquitectura: Events/EventChannels
 * Script: GameplayEventChannelsBinding
 * Rol: Composition root de canales de eventos por escena.
 * Relaciones: Configura los buses estaticos para que publiquen tambien en EventChannels ScriptableObject.
 * Uso como referencia: mantener este binding en escena evita singletons y hace visibles las dependencias de eventos.
 */
using UnityEngine;

public class GameplayEventChannelsBinding : MonoBehaviour
{
    [Header("Core Channels")]
    [SerializeField] private NotificationEventChannel_SO notificationChannel;
    [SerializeField] private NotificationStateEventChannel_SO notificationStateChannel;
    [SerializeField] private GameStateEventChannel_SO gameStateChannel;
    [SerializeField] private UIStateEventChannel_SO uiStateChannel;

    [Header("System Channels")]
    [SerializeField] private InventoryEventChannel_SO inventoryChannel;
    [SerializeField] private CraftingEventChannel_SO craftingChannel;
    [SerializeField] private QuestEventChannel_SO questChannel;
    [SerializeField] private ScannerFeedbackEventChannel_SO scannerFeedbackChannel;
    [SerializeField] private WeatherStateEventChannel_SO weatherStateChannel;

    private void Awake()
    {
        NotificationEvents.ConfigureChannels(notificationChannel, notificationStateChannel);
        GameStateEvents.ConfigureChannel(gameStateChannel);
        UIStateEvents.ConfigureChannel(uiStateChannel);
        InventoryEvents.ConfigureChannel(inventoryChannel);
        CraftingEvents.ConfigureChannel(craftingChannel);
        QuestEvents.ConfigureChannel(questChannel);
        ScannerEvents.ConfigureChannel(scannerFeedbackChannel);
        WeatherEvents.ConfigureChannel(weatherStateChannel);
    }
}
