/*
 * Arquitectura: Menu/Core
 * Script: ISceneLoader
 * Rol: Contrato de carga de escenas para UI, SaveLoad y flujo de juego.
 * Relaciones: MainMenuManager, PauseMenuManager, MenuController y SaveLoadSceneBinding consumen este contrato en lugar de SceneManager directo.
 * Riesgo arquitectonico mitigado: centraliza la forma de cargar escenas y evita nombres/indices repartidos por sistemas.
 */
public interface ISceneLoader
{
    void LoadScene(string sceneName);
    void LoadScene(int sceneBuildIndex);
}
