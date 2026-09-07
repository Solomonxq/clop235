using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Завантажуємо наступну сцену в порядку побудови
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Вихід з гри"); // Для перевірки в редакторі
    }
}
