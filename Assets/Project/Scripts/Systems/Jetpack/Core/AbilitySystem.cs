/*
 * Arquitectura: Jetpack/Core
 * Script: AbilitySystem
 * Rol: Contiene reglas de dominio reutilizables. Debe evitar referencias directas a UI y depender de interfaces cuando colabora con otros sistemas.
 * Modulo: Gestiona combustible, empuje y habilidades asociadas al propulsor, colaborando con Movement mediante una interfaz.
 * Relaciones: Usa IJetpackMovementContext para aplicar fuerzas sin depender de MovementController concreto.
 * Uso como referencia: este comentario explica la responsabilidad del archivo para facilitar estudiar y replicar la arquitectura modular en otros proyectos.
 */
using System.Collections.Generic;

public class AbilitySystem
{
    private List<IAbility> abilities = new List<IAbility>();

    public void Register(IAbility ability)
    {
        abilities.Add(ability);
    }

    public void Tick(float deltaTime)
    {
        foreach (var ability in abilities)
        {
            ability.Tick(deltaTime);
        }
    }
}