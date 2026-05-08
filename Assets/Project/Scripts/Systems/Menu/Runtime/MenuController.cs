using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // 🔥 SOLO ESTA FUNCIÓN
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Controllers (loading)
    }
}