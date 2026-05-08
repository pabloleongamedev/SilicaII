/*
 * Arquitectura: Inventory/Debug
 * Script: Deprecado_InventoryTester
 * Rol: Apoyo para depuracion, documentacion o pruebas manuales. No debe ser dependencia de gameplay de produccion.
 * Modulo: Gestiona items, cantidades, slots, vistas de inventario y contratos de lectura/escritura para otros sistemas.
 * Relaciones: Se relaciona con Interaction, Crafting, Delivery, Quest y SaveLoad mediante interfaces, facades y eventos.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[Obsolete("Herramienta manual de debug. No usar en escenas de produccion; reemplazar por tests automatizados del dominio.")]
public class InventoryTester : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;

    [Header("Test Items")]
    [SerializeField] private ItemData_SO testItemA;
    [SerializeField] private ItemData_SO testItemB;

    private IInventoryReadModel readModel;
    private IInventoryWriteModel writeModel;

    private void Start()
    {
        readModel = inventoryController.ReadModel;
        writeModel = inventoryController.WriteModel;
    }

    //private void Update()
   /* {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            Test_EmptyInventory();

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            Test_Stacking();

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            Test_FullInventory();

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            Test_BreakdownScenario();

        if (Keyboard.current.digit5Key.wasPressedThisFrame)
            Test_PartialInsertProtection();
    }*/

    // -------------------------
    // CORE
    // -------------------------

    void ResetInventory()
    {
        inventoryController.ResetInventory();
    }

    void FillInventoryCompletely(ItemData_SO item)
    {
        while (readModel.CanAddItem(item, item.maxStack))
        {
            writeModel.AddItem(item, item.maxStack);
        }
    }

    // -------------------------
    // TESTS
    // -------------------------

    void Test_EmptyInventory()
    {
        ResetInventory();

        Debug.Log("=== TEST 1: EMPTY ===");

        bool canAdd = readModel.CanAddItem(testItemA, 10);
        int added = writeModel.AddItem(testItemA, 10);

        Debug.Log($"CanAdd: {canAdd}");
        Debug.Log($"Added: {added}");

        if (!canAdd || added != 10)
            Debug.LogError("❌ ERROR en inventario vacío");
        else
            Debug.Log("✅ OK");
    }

    void Test_Stacking()
    {
        ResetInventory();

        Debug.Log("=== TEST 2: STACKING ===");

        writeModel.AddItem(testItemA, 95);

        bool canAddSmall = readModel.CanAddItem(testItemA, 4);
        int addedSmall = writeModel.AddItem(testItemA, 4);

        bool canAddLarge = readModel.CanAddItem(testItemA, 10);
        int addedLarge = writeModel.AddItem(testItemA, 10);

        Debug.Log($"CanAdd (4): {canAddSmall} | Added: {addedSmall}");
        Debug.Log($"CanAdd (10): {canAddLarge} | Added: {addedLarge}");

        if (!canAddSmall || addedSmall != 4)
            Debug.LogError("❌ ERROR stacking pequeño");

        if (!canAddLarge || addedLarge != 10)
            Debug.LogError("❌ ERROR stacking grande");
    }

    void Test_FullInventory()
    {
        ResetInventory();

        Debug.Log("=== TEST 3: FULL INVENTORY ===");

        FillInventoryCompletely(testItemA);

        bool canAdd = readModel.CanAddItem(testItemA, 1);
        int added = writeModel.AddItem(testItemA, 1);

        Debug.Log($"CanAdd: {canAdd}");
        Debug.Log($"Added: {added} (should be 0)");

        if (canAdd || added > 0)
            Debug.LogError("❌ ERROR: Inventario lleno pero permitió inserción");
        else
            Debug.Log("✅ OK: bloqueo correcto");
    }

    void Test_BreakdownScenario()
    {
        Debug.Log("=== TEST 4: BATCH SIMULATION ===");

        var inventory = inventoryController.GetInventorySystem();

        bool canBatch = inventory.CanAddItemsBatch(
            (testItemA, 15),
            (testItemB, 5)
        );

        Debug.Log($"CanAdd Batch (A15 + B5): {canBatch}");

        if (!canBatch)
        {
            Debug.Log("✅ Correcto: el sistema detecta conflicto real de espacio");
        }
        else
        {
            Debug.Log("⚠️ Hay espacio suficiente para ambos");
        }
    }

    void Test_PartialInsertProtection()
    {
        ResetInventory();

        Debug.Log("=== TEST 5: PARTIAL INSERT PROTECTION ===");

        FillInventoryCompletely(testItemA);

        int bigAmount = 999;

        bool canAdd = readModel.CanAddItem(testItemA, bigAmount);
        int added = writeModel.AddItem(testItemA, bigAmount);

        Debug.Log($"CanAdd: {canAdd}");
        Debug.Log($"Added: {added}");

        if (!canAdd && added > 0)
        {
            Debug.LogError("❌ BUG CRÍTICO: inserción parcial ocurrió");
        }
        else if (!canAdd && added == 0)
        {
            Debug.Log("✅ Correcto: bloqueo total");
        }
        else if (canAdd && added == bigAmount)
        {
            Debug.Log("✅ Inserción completa");
        }
        else
        {
            Debug.LogWarning("⚠️ Estado inconsistente");
        }
    }
}
