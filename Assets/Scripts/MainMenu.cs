using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Time.timeScale = 1f; // ← ВАЖНО
        SceneManager.LoadScene("SampleScene"); // ВАЖНО: имя сцены игры
    }

    public void QuitGame()
    {
        
        Debug.Log("Выход из игры");

        Application.Quit();
    }
}