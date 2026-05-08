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