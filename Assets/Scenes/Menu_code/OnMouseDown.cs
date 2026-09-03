using UnityEngine;

public class ButtonClickHandler : MonoBehaviour
{
    public string sceneToLoad; // Назва сцени, куди перейти
    public bool isExitButton = false; // Чи це кнопка виходу

    private void OnMouseDown()
    {
        // Шукаємо наш GameManager зі скриптом SceneLoader на сцені
        SceneLoader loader = FindFirstObjectByType<SceneLoader>();

        if (loader != null)
        {
            if (isExitButton)
            {
                loader.QuitGame();
            }
            else if (!string.IsNullOrEmpty(sceneToLoad))
            {
                loader.LoadSceneByName(sceneToLoad);
            }
        }
    }
}