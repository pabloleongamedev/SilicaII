/*
 * Arquitectura: Systems
 * Script: ArchitectureRules
 * Rol: Documento vivo de reglas base para estudiar y replicar la arquitectura modular del proyecto.
 * Fase 0:
 * - Core contiene reglas puras y no debe buscar objetos de escena, UI ni singletons.
 * - Data define assets/DTOs serializables y no ejecuta flujo de gameplay.
 * - Runtime adapta Unity a Core, orquesta dependencias explicitas y publica eventos del dominio.
 * - UI presenta estado y captura intenciones visuales; no decide dano, guardado, misiones ni inventario.
 * - SaveLoad persiste DTOs mediante ISaveParticipant; cada sistema exporta/restaura su propia parte.
 * - Events se separa por dominio mediante EventChannels ScriptableObject asignados por Inspector.
 * - Las referencias por Inspector son la ruta principal; FindFirstObjectByType no debe usarse en codigo productivo.
 */
public static class ArchitectureRules
{
}
