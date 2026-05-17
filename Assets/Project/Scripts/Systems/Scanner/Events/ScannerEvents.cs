/*
 * Arquitectura: Scanner/Events
 * Script: ScannerEvents
 * Rol: Canal de dominio del scanner para pedir feedback visual sin acoplar objetos escaneables al Player.
 * Relaciones: ScannableObject publica OnScanFeedbackRequested; ScannerTrigger escucha y ejecuta animacion/audio.
 * Riesgo arquitectonico mitigado: el objeto del mundo no conoce ScannerTrigger, AudioManager ni jerarquia del Player.
 */
using System;
using UnityEngine;

public static class ScannerEvents
{
    public static event Action<IScannable> OnScanFeedbackRequested;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        OnScanFeedbackRequested = null;
    }

    public static void RequestScanFeedback(IScannable scannable)
    {
        OnScanFeedbackRequested?.Invoke(scannable);
    }
}
