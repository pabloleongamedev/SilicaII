/*
 * Arquitectura: Events/Input
 * Script: InputActivityEvents
 * Rol: Canal explicito para actividad de input del jugador.
 * Relaciones: PlayerInputHandler publica actividad; sistemas de tutorial/ayuda pueden observar sin acoplarse al input concreto.
 * Fase 2: separa input de los eventos de UI, notificacion y estado de juego.
 */
using System;

public static class InputActivityEvents
{
    public static Action OnAnyInput;
}
