/*
 * Arquitectura: SaveLoad/Runtime
 * Script: AutosaveController
 * Rol: Controla temporizacion de autosave sin conocer disco, escena ni participantes.
 * Relaciones: GameManager lo alimenta con deltaTime y ejecuta SaveService cuando ShouldSave retorna true.
 */
public class AutosaveController
{
    private readonly float interval;
    private float elapsed;

    public AutosaveController(float interval)
    {
        this.interval = interval;
    }

    public void Reset()
    {
        elapsed = 0f;
    }

    public bool Tick(float deltaTime)
    {
        if (interval <= 0f)
            return false;

        elapsed += deltaTime;

        if (elapsed < interval)
            return false;

        elapsed = 0f;
        return true;
    }
}
