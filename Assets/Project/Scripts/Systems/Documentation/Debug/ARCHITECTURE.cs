/**
 * 🎮 ARQUITECTURA DEL SISTEMA DE GUARDADO AUTOMÁTICO
 * 
 * Este archivo documenta la arquitectura completa del sistema implementado
 */

/*
================================================================================
                            DIAGRAMA DE CLASES
================================================================================

┌─────────────────────────────────────────────────────────────────────────┐
│                         GAMEMANAGER (Singleton)                         │
├─────────────────────────────────────────────────────────────────────────┤
│ • SaveController: Maneja I/O                                            │
│ • GameData currentGameData: Datos actuales en memoria                   │
│ • float autoSaveInterval: Cada 60s                                      │
│ • bool isInGame: Detecta si está jugando                                │
├─────────────────────────────────────────────────────────────────────────┤
│ PUBLIC METHODS:                                                         │
│ • LoadGame(slotID): Cargar una partida                                  │
│ • CreateNewGame(slotID): Nueva partida                                  │
│ • SaveGame(): Guardar manualmente                                       │
│ • HasSaveFile(slotID): ¿Existe guardado?                               │
│ • UpdatePlayerPosition(Vector3)                                         │
│ • UpdatePlayerRotation(Quaternion)                                      │
│ • RegisterScannedElement(elementID)                                     │
└─────────────────────────────────────────────────────────────────────────┘
              ↓                           ↓                    ↓
     ┌──────────────────┐    ┌──────────────────┐  ┌──────────────────┐
     │  SAVECONTROLLER  │    │   GAMEDATA       │  │  GAMRESTORER     │
     │  (Lee/Escribe)   │    │  (Serializable)  │  │  (Restaura)      │
     └──────────────────┘    └──────────────────┘  └──────────────────┘

================================================================================
                        DIAGRAMA DE FLUJO - INICIO DE JUEGO
================================================================================

    ┌─────────────────┐
    │  ABRIR JUEGO    │
    └────────┬────────┘
             │
             ↓
    ┌──────────────────────────────┐
    │ MainMenu Scene Cargada       │
    │ • GameManager creado         │
    │ • Singletonizado inmediato   │
    └────────┬─────────────────────┘
             │
             ↓ (Usuario presiona "Jugar")
    ┌──────────────────────────────┐
    │ MainMenuManager.ShowPlayMenu()│
    │ • Refresca SaveSlots          │
    └────────┬─────────────────────┘
             │
             ↓
    ┌──────────────────────────────────────┐
    │ SaveSlot.RefreshSlot() x 3           │
    │ • Pregunta a GameManager.HasSaveFile │
    │ • Obtiene SaveInfo                   │
    │ • Actualiza visual                   │
    └────────┬─────────────────────────────┘
             │
             ↓
    ╔════════════════════════════════════════╗
    ║   USUARIO VE:                          ║
    ║   Slot 1 - Continuar (último guardado) ║
    ║   Slot 2 - Continuar (guardado previo) ║
    ║   Slot 3 - Nueva Partida               ║
    ╚════════════════════════════════════════╝


================================================================================
                    DIAGRAMA DE FLUJO - SELECCIONAR SLOT
================================================================================

                     Usuario Presiona SaveSlot
                              │
                              ↓
    ┌──────────────────────────────────────────────┐
    │ SaveSlot.OnSlotPressed()                     │
    └──────────────────┬───────────────────────────┘
                       │
           ┌───────────┴────────────┐
           ↓                        ↓
    ¿Tiene datos?             No tiene datos
           │YES                     │NO
           ↓                        ↓
    ┌──────────────────────┐  ┌──────────────────┐
    │ GameManager.         │  │ GameManager.     │
    │ LoadGame(slotID)     │  │ CreateNewGame()  │
    └──────────┬───────────┘  └────┬─────────────┘
               │                   │
               ↓                   ↓
    ┌──────────────────────┐  ┌──────────────────┐
    │ SaveController.      │  │ GameData.Create  │
    │ LoadGame(slotID)     │  │ NewGame()        │
    │ • Lee JSON del disco │  │ • Datos por def. │
    │ • Deserializa        │  │ • Pos (0,1,0)   │
    └──────────┬───────────┘  └────┬─────────────┘
               │                   │
               ↓                   ↓
    SceneManager.LoadScene(sceneGuardada)
               │
               ↓
    ┌──────────────────────────────────────┐
    │ GameScene Cargada                    │
    │ • Ejecuta Awake()                    │
    └──────────┬───────────────────────────┘
               │
               ↓
    ┌──────────────────────────────────────┐
    │ GameRestorer.Awake()                 │
    │ • Obtiene GameData                   │
    │ • Restaura posición del jugador      │
    │ • Restaura rotación del jugador      │
    │ • Debug log de restauración          │
    └──────────┬───────────────────────────┘
               │
               ↓
    ╔════════════════════════════════════════╗
    ║   JUEGO CONTINÚA DONDE QUEDÓ          ║
    ║   (Si cargó partida guardada)         ║
    ╚════════════════════════════════════════╝


================================================================================
                    DIAGRAMA DE FLUJO - AUTO-SAVE EN JUEGO
================================================================================

    GameManager.Update() [CADA FRAME]
           │
           ↓
    ¿isInGame && enableAutoSave?
           │
      ┌────┴─────┐
      ↓          ↓
    YES         NO → Saltar
      │
      ↓
    timeSinceLastSave += Time.deltaTime
      │
      ↓
    ¿timeSinceLastSave >= autoSaveInterval?
           │
      ┌────┴─────┐
      ↓          ↓
    YES         NO → Esperar
      │
      ↓
    ┌─────────────────────────────┐
    │ AutoSave()                  │
    │ • UpdatePlayerData()        │
    │ • SaveController.SaveGame() │
    │ • Escribir JSON a disco     │
    │ • Debug.Log("AUTO-SAVE")    │
    └─────────────────────────────┘
      │
      ↓
    timeSinceLastSave = 0f
      │
      ↓
    Volver a siguiente ciclo


================================================================================
                    SINCRONIZACIÓN CON GAMADOATA
================================================================================

PlayerController.Update()
    │
    ├─→ Si GameManager.Instance existe
    │   │
    │   └─→ Cada 0.5 segundos:
    │       • GameManager.UpdatePlayerPosition(transform.position)
    │       • GameManager.UpdatePlayerRotation(transform.rotation)
    │
    └─→ (Los datos se incorporan en el próximo auto-save)


Otros Sistemas:
    │
    ├─→ InventorySystem
    │   └─→ GameManager.AddInventoryItem(itemID, gridX, gridY)
    │
    ├─→ ScanSystem
    │   └─→ GameManager.RegisterScannedElement(elementID)
    │
    └─→ PlayerHealth
        └─→ GameManager.UpdatePlayerHealth(hp, maxHp)


================================================================================
                         ESTRUCTURA DE GAMEDATA
================================================================================

GameData
├── Información de Partida
│   ├── slotID: "1"
│   ├── lastSaveTime: "2026-04-14 15:30:45"
│   └── playTimeSeconds: 5023
│
├── Datos del Jugador (PlayerSaveData)
│   ├── Posición: posX, posY, posZ
│   ├── Rotación: rotX, rotY, rotZ, rotW
│   ├── health: 100
│   ├── maxHealth: 100
│   └── jetpackFuel: 100
│
├── Inventario (List<InventorySaveData>)
│   ├── [0] itemID: "sword_01", gridX: 0, gridY: 0
│   ├── [1] itemID: "shield_01", gridX: 1, gridY: 0
│   └── [N] ...
│
├── Progreso
│   ├── currentScene: "GameScene"
│   └── scannedElements: ["element_1", "element_2"]
│
└── Estado del Mundo
    ├── collectedItems: ["item_1", "item_2"]
    └── destroyedObjects: ["enemy_01", "obstacle_03"]


================================================================================
                    CARPETA FISICA DE GUARDOS
================================================================================

Application.persistentDataPath
└── Saves/
    ├── save_1.json  [Contenido JSON legible]
    ├── save_2.json  [Contenido JSON legible]
    └── save_3.json  [Contenido JSON legible]

Ejemplo de save_1.json:
{
  "slotID": "1",
  "lastSaveTime": "2026-04-14 15:30:45",
  "playTimeSeconds": 5023,
  "playerData": {
    "posX": 10.5,
    "posY": 2.0,
    "posZ": -5.3,
    "rotX": 0.0,
    "rotY": 0.707,
    "rotZ": 0.0,
    "rotW": 0.707,
    "health": 100,
    "maxHealth": 100,
    "jetpackFuel": 75.5
  },
  "inventoryItems": [...],
  "currentScene": "GameScene",
  "scannedElements": [...],
  "collectedItems": [...],
  "destroyedObjects": [...]
}


================================================================================
                    PUNTOS CLAVE DE INTEGRACIÓN
================================================================================

1. INICIALIZACIÓN
   └─→ MainMenu Scene: Crear GameObject con GameManager.cs
   └─→ Se crea como Singleton automáticamente

2. MENÚ DE INICIO
   └─→ SaveSlot.RefreshSlot() se llama cuando se abre panel "Jugar"
   └─→ Detecta saves y muestra "Continuar" o "Nueva Partida"

3. CARGA DE JUEGO
   └─→ SaveSlot.OnSlotPressed() → GameManager.LoadGame() o CreateNewGame()
   └─→ GameRestorer restaura posición/rotación en Awake()

4. DURANTE EL JUEGO
   └─→ PlayerController sincroniza posición cada 0.5s
   └─→ GameManager auto-guarda cada 60s (configurable)
   └─→ Otros sistemas agregan data via GameManager.Update*()

5. SISTEMAS EXTERNOS
   └─→ InventorySystem → AddInventoryItem()
   └─→ ScanSystem → RegisterScannedElement()
   └─→ PlayerHealth → UpdatePlayerHealth()


================================================================================
                        BENEFICIOS DE ESTA ARQUITECTURA
================================================================================

✅ TRANSPARENTE: Auto-save sin intervención del usuario
✅ ESCALABLE: Agregar nuevos datos sin cambiar la estructura central
✅ SEGURO: Datos persistentes en JSON
✅ ACCESIBLE: GameManager.Instance desde cualquier script
✅ DEBUGGABLE: Logs detallados + DebugGetGameState()
✅ MULTIPLATAFORMA: Application.persistentDataPath compatible
✅ SIN DEPENDENCIAS: JsonUtility es nativo de Unity
✅ FLEXIBLE: Múltiples slots con cargas independientes

*/
