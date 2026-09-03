using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Функція для завантаження сцени (можна викликати з кнопки або через клік)
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Вихід з гри
    public void QuitGame()
    {
        Debug.Log("Гра закривається...");
        Application.Quit();
    }
}