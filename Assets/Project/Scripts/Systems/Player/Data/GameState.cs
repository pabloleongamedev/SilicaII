/*
 * Arquitectura: Player/Data
 * Script: GameState
 * Rol: Define datos editables o estructuras serializables. No debe ejecutar reglas de gameplay ni controlar escena.
 * Modulo: Gestiona estado global del jugador, input y bloqueos de gameplay/UI.
 * Relaciones: Coordina input, estados UI/gameplay y bloqueos globales usados por otros sistemas; PlayerDeathHandler usa GameOver cuando la vida del jugador llega a cero.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
public enum GameState
{
    Gameplay,
    Blocked,
    Cinematic,
    GameOver
}
