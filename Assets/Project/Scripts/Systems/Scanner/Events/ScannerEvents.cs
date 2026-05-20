/*
 * Arquitectura: Scanner/Events
 * Script: ScannerEvents
 * Rol: Canal de dominio del scanner para pedir feedback visual sin acoplar objetos escaneables al Player.
 * Relaciones: ItemPickup en modo Scannable publica OnScanFeedbackRequested; ScannerTrigger escucha y ejecuta animacion/audio.
 * Riesgo arquitectonico mitigado: el objeto del mundo no conoce ScannerTrigger, AudioService ni jerarquia del Player.
 */
using System;
using UnityEngine;

public static class ScannerEvents
{
    public static event Action<IScannable> OnScanFeedbackRequested;
    private static ScannerFeedbackEventChannel_SO channel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        OnScanFeedbackRequested = null;
        channel = null;
    }

    public static void ConfigureChannel(ScannerFeedbackEventChannel_SO eventChannel)
    {
        channel = eventChannel;
    }

    public static void RequestScanFeedback(IScannable scannable)
    {
        OnScanFeedbackRequested?.Invoke(scannable);
        channel?.Raise(scannable);
    }
}
