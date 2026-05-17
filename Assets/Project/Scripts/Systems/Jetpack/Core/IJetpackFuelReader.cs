/*
 * Arquitectura: Jetpack/Core
 * Script: IJetpackFuelReader
 * Rol: Contrato de lectura para UI/SaveLoad. Expone combustible sin revelar JetpackSystem ni MovementController.
 * Relaciones: MovementController implementa esta interfaz; JetpackHUDPresenter observa el ratio sin acoplarse al sistema de vuelo.
 */
using System;

public interface IJetpackFuelReader
{
    event Action<float> OnFuelRatioChanged;

    float GetFuelRatio();

    float GetCurrentFuel();
}
