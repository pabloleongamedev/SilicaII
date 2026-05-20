/*
 * Arquitectura: Audio/Core
 * Script: AudioCueKey
 * Rol: Identificador tipado de intenciones de audio sin strings sueltos ni referencias duplicadas por sistema.
 * Relaciones: Los consumidores piden una clave; AudioService la resuelve en AudioCueLibrary_SO.
 */
public enum AudioCueKey
{
    None = 0,

    PlayerWalk = 10,
    PlayerMetalWalk = 11,
    PlayerJump = 12,
    PlayerMetalJump = 13,
    PlayerJetpack = 14,

    ScannerScan = 30,

    NotificationInfo = 50,
    NotificationSuccess = 51,
    NotificationWarning = 52,
    NotificationError = 53,

    UIHover = 70,
    UIClick = 71,
    UIApply = 72,
    UIReset = 73,
    UISliderTick = 74,
    UISliderLimit = 75,
    UIError = 76,

    FeedbackHit = 100,
    FeedbackExplosion = 101,
    FeedbackPickup = 102
}
