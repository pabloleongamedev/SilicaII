/**
 * EJEMPLOS DE EXTENSIÓN - Cómo agregar nuevos datos al sistema de guardado
 * 
 * Este archivo contiene ejemplos prácticos de cómo integrar otros sistemas
 * con el GameManager para guardar sus datos automáticamente.
 */

// ============================================================================
// EJEMPLO 1: AGREGAR ITEMS AL INVENTARIO
// ============================================================================

/*
UBICACIÓN: Assets/Project/Scripts/Systems/Inventory/InventorySystem.cs

ANTES:
    public bool AddItem(ItemData_SO itemData)
    {
        var instance = new InventoryItemInstance(itemData);
        bool result = grid.TryAddItem(instance);
        
        if (result)
        {
            Debug.Log($"Item agregado: {itemData.displayName}");
        }
        
        return result;
    }

DESPUÉS (Integración con GameManager):
    public bool AddItem(ItemData_SO itemData)
    {
        var instance = new InventoryItemInstance(itemData);
        bool result = grid.TryAddItem(instance);
        
        if (result)
        {
            Debug.Log($"Item agregado: {itemData.displayName}");
            
            // ↓↓↓ NUEVO: Guardar en GameManager ↓↓↓
            if (GameManager.Instance != null)
            {
                // Obtener posición en el grid
                grid.TryGetItemPosition(instance, out int gridX, out int gridY);
                GameManager.Instance.AddInventoryItem(
                    itemData.itemID,
                    gridX,
                    gridY,
                    itemData.cantidad
                );
            }
            // ↑↑↑ FIN NUEVO ↑↑↑
        }
        
        return result;
    }
*/

// ============================================================================
// EJEMPLO 2: REGISTRAR ELEMENTOS ESCANEADOS
// ============================================================================

/*
UBICACIÓN: Assets/Project/Scripts/Systems/Scanner/ScanSystem.cs

ANTES:
    public void Scan(IScannable scannable)
    {
        scannedItems.Add(scannable);
        Debug.Log($"Elemento escaneado: {scannable.GetName()}");
    }

DESPUÉS (Integración con GameManager):
    public void Scan(IScannable scannable)
    {
        scannedItems.Add(scannable);
        Debug.Log($"Elemento escaneado: {scannable.GetName()}");
        
        // ↓↓↓ NUEVO: Guardar en GameManager ↓↓↓
        if (GameManager.Instance != null)
        {
            // Asumir que IScannable tiene property ID
            GameManager.Instance.RegisterScannedElement(
                scannable.GetID() ?? scannable.GetName()
            );
        }
        // ↑↑↑ FIN NUEVO ↑↑↑
    }
*/

// ============================================================================
// EJEMPLO 3: AGREGAR SALUD DEL JUGADOR
// ============================================================================

/*
UBICACIÓN: (nuevo script) Assets/Project/Scripts/Systems/Player/PlayerHealth.cs

CREAR NUEVO SCRIPT:

using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateGameManager();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        UpdateGameManager();
    }

    private void UpdateGameManager()
    {
        // ↓↓↓ NUEVO: Guardar health en GameManager ↓↓↓
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdatePlayerHealth(
                currentHealth,
                maxHealth
            );
        }
        // ↑↑↑ FIN NUEVO ↑↑↑
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto");
        // Lógica de muerte...
    }
}
*/

// ============================================================================
// EJEMPLO 4: AGREGAR DATO PERSONALIZADO (Nivel de Competencia)
// ============================================================================

/*
PASO 1: Extender GameData en GameData.cs

ANTES:
    public class GameData
    {
        public string slotID = "1";
        public PlayerSaveData playerData = new PlayerSaveData();
        public List<InventorySaveData> inventoryItems = new List<InventorySaveData>();
        // ... más campos ...
    }

DESPUÉS:
    public class GameData
    {
        public string slotID = "1";
        public PlayerSaveData playerData = new PlayerSaveData();
        public List<InventorySaveData> inventoryItems = new List<InventorySaveData>();
        
        // ↓↓↓ NUEVO: Datos personalizados ↓↓↓
        [Header("Player Skills")]
        public int scanningLevel = 1;      // Nivel de escaneo
        public int jumpCount = 0;          // Saltos realizados
        public float totalDistanceTraveled = 0;
        // ↑↑↑ FIN NUEVO ↑↑↑
        
        // ... más campos ...
    }

PASO 2: Agregar métodos de actualización en GameManager

ANTES:
    public void UpdatePlayerHealth(int health, int maxHealth)
    {
        if (currentGameData != null)
        {
            currentGameData.playerData.health = health;
            currentGameData.playerData.maxHealth = maxHealth;
        }
    }

DESPUÉS:
    public void UpdatePlayerHealth(int health, int maxHealth)
    {
        if (currentGameData != null)
        {
            currentGameData.playerData.health = health;
            currentGameData.playerData.maxHealth = maxHealth;
        }
    }
    
    // ↓↓↓ NUEVO: Métodos para datos personalizados ↓↓↓
    public void UpdateScanningLevel(int level)
    {
        if (currentGameData != null)
        {
            currentGameData.scanningLevel = level;
        }
    }
    
    public void IncrementJumpCount()
    {
        if (currentGameData != null)
        {
            currentGameData.jumpCount++;
        }
    }
    
    public void AddDistanceTraveled(float distance)
    {
        if (currentGameData != null)
        {
            currentGameData.totalDistanceTraveled += distance;
        }
    }
    // ↑↑↑ FIN NUEVO ↑↑↑

PASO 3: Usar en los scripts correspondientes

// En ScannerController.cs:
public void TryScan()
{
    Ray ray = Camera.main.ViewportPointToRay(Vector3.forward);
    if (Physics.Raycast(ray, out RaycastHit hit, 3f))
    {
        var scannable = hit.collider.GetComponent<IScannable>();
        if (scannable != null)
        {
            scanSystem.Scan(scannable);
            
            // ↓↓↓ NUEVO: Incrementar nivel ↓↓↓
            if (GameManager.Instance != null)
            {
                // Cada 5 escaneos, subir nivel
                if (scanSystem.GetScannedCount() % 5 == 0)
                {
                    GameManager.Instance.UpdateScanningLevel(
                        scanSystem.GetScannedCount() / 5
                    );
                }
            }
            // ↑↑↑ FIN NUEVO ↑↑↑
        }
    }
}

// En PlayerController.cs (en OnJumpStarted):
public void OnJumpStarted()
{
    movementController.OnJumpStarted();
    
    // ↓↓↓ NUEVO: Registrar salto ↓↓↓
    if (GameManager.Instance != null)
    {
        GameManager.Instance.IncrementJumpCount();
    }
    // ↑↑↑ FIN NUEVO ↑↑↑
}

// En MovementController.cs (cada frame de movimiento):
private void Update()
{
    // ... código de movimiento ...
    
    // ↓↓↓ NUEVO: Registrar distancia ↓↓↓
    float distance = Vector3.Distance(lastPosition, transform.position);
    if (GameManager.Instance != null && distance > 0)
    {
        GameManager.Instance.AddDistanceTraveled(distance);
    }
    lastPosition = transform.position;
    // ↑↑↑ FIN NUEVO ↑↑↑
}
*/

// ============================================================================
// EJEMPLO 5: CARGAR DATOS PERSONALIZADOS AL INICIAR PARTIDA
// ============================================================================

/*
UBICACIÓN: GameRestorer.cs (extensión)

ANTES:
    private void Awake()
    {
        GameData gameData = GameManager.Instance?.GetCurrentGameData();
        if (gameData == null) return;

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            Transform playerTransform = playerController.transform;
            playerTransform.position = gameData.playerData.GetPosition();
            playerTransform.rotation = gameData.playerData.GetRotation();
        }
    }

DESPUÉS:
    private void Awake()
    {
        GameData gameData = GameManager.Instance?.GetCurrentGameData();
        if (gameData == null) return;

        // Restaurar posición
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            Transform playerTransform = playerController.transform;
            playerTransform.position = gameData.playerData.GetPosition();
            playerTransform.rotation = gameData.playerData.GetRotation();
        }

        // ↓↓↓ NUEVO: Restaurar sistemas ↓↓↓
        
        // Restaurar salud
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetHealth(gameData.playerData.health);
        }

        // Restaurar nivel de escaneo
        ScannerController scanner = FindObjectOfType<ScannerController>();
        if (scanner != null)
        {
            scanner.SetScanLevel(gameData.scanningLevel);
        }

        // Restaurar inventario
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory != null)
        {
            foreach (var itemData in gameData.inventoryItems)
            {
                // Cargar cada item del JSON
                // (requiere que InventorySystem pueda cargar desde ItemData)
                inventory.RestoreItem(itemData);
            }
        }
        
        // ↑↑↑ FIN NUEVO ↑↑↑
    }
*/

// ============================================================================
// EJEMPLO 6: PATRÓN GENERAL
// ============================================================================

/*
PATRÓN UNIVERSAL PARA CUALQUIER SISTEMA:

PASO 1: Extender GameData (si es necesario)
    → Agregar campo público [System.Serializable]
    → Setter/Getter si es necesario

PASO 2: Extender GameManager
    → public void UpdateMySystem(param)
    → Guardar en currentGameData.myField

PASO 3: En el Sistema (script que genera data)
    → if (GameManager.Instance != null)
    →     GameManager.Instance.UpdateMySystem(data);

PASO 4: En GameRestorer (si necesita restaurarse)
    → if (mySystem != null)
    →     mySystem.Load(gameData.myField);

EJEMPLO MÍNIMO:

// GameData.cs:
[System.Serializable]
public class GameData
{
    public MyCustomData customData = new MyCustomData();
}

[System.Serializable]
public class MyCustomData
{
    public int myValue;
}

// GameManager.cs:
public void UpdateCustomData(int value)
{
    if (currentGameData != null)
        currentGameData.customData.myValue = value;
}

// MySystem.cs
public void DothSomething()
{
    GameManager.Instance?.UpdateCustomData(42);
}

// GameRestorer.cs
MySystem system = FindObjectOfType<MySystem>();
if (system != null)
    system.SetValue(gameData.customData.myValue);
*/

// ============================================================================
// CHECKLIST PARA AGREGAR NUEVO DATO
// ============================================================================

/*
Para agregar un nuevo dato al sistema, sigue estos pasos:

□ 1. ¿Es serializable?
    ├─ Sí → Continúa
    └─ No → Hacer que sea serializable ([System.Serializable])

□ 2. Extender GameData.cs
    ├─ Agregar field público
    ├─ Que sea [System.Serializable] si es clase
    └─ Con comentario describiendo qué es

□ 3. Extender GameManager.cs
    ├─ Crear public void Update{NombreDato}(param)
    ├─ Guardar en currentGameData.{field}
    └─ Si es complejo, crear multiple Updates

□ 4. Integrar en el Sistema
    ├─ Buscar sección de código que genera el dato
    ├─ Agregar: if (GameManager.Instance != null)
    ├─ Llamar: GameManager.Instance.Update{NombreDato}(value)
    └─ Probar que el dato se guarda (ver en JSON)

□ 5. Restaurar (Opcional)
    ├─ Si necesita restaurarse al cargar:
    ├─ Extender GameRestorer.cs
    ├─ Obtener el dato: gameData.{field}
    ├─ Restaurar: system.SetValue(value)
    └─ Probar que se restaura correctamente

□ 6. Testing
    ├─ Crear partida nueva
    ├─ Generar el dato
    ├─ Esperar auto-save (60s)
    ├─ Abrir archivo save_X.json
    ├─ Verificar que el dato está en JSON
    ├─ Cargar partida guardada
    ├─ Verificar que se restaura correctamente
    └─ ¡Listo!
*/

// ============================================================================
// REFERENCIAS RÁPIDAS
// ============================================================================

/*
Métodos públicos del GameManager:

Guardado:
  • SaveGame()
  • LoadGame(slotID)
  • CreateNewGame(slotID)
  • HasSaveFile(slotID)

Jugador:
  • UpdatePlayerPosition(Vector3)
  • UpdatePlayerRotation(Quaternion)
  • UpdatePlayerHealth(health, maxHealth)

Inventario:
  • AddInventoryItem(itemID, gridX, gridY, quantity)

Progreso:
  • RegisterScannedElement(elementID)

Debugging:
  • DebugGetGameState()
  • GetCurrentGameData()
  • GetCurrentSlotID()

Getters:
  • GetSaveInfo(slotID)
  • GetAllSaveInfos()
  • RefreshSaveStates()
*/

