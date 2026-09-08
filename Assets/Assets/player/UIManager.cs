using UnityEngine;
using UnityEngine.UI;
using TMPro; // Обов'язково для TextMeshPro

public class UIManager : MonoBehaviour
{
    public PlayerStats playerStats; 
    
    [Header("Смужки UI")]
    public Slider healthSlider;
    public Slider staminaSlider;

    [Header("UI Рівня та Досвіду")]
    public TextMeshProUGUI levelText;      // Текст для рівня
    public TextMeshProUGUI expText;        // Текст для досвіду (наприклад, "50 / 100")

    void OnEnable()
    {
        // 1. Якщо гравець не перетягнутий в Інспекторі, шукаємо його на сцені автоматично
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
        }

        // 2. Якщо скрипт успішно знайшов гравця, підписуємося на події
        if (playerStats != null)
        {
            playerStats.OnHealthChanged += UpdateHealthUI;
            playerStats.OnStaminaChanged += UpdateStaminaUI;
            playerStats.OnLevlChanged += UpdateLevelUI;
            playerStats.OnExpChanged += UpdateExpUI;
            playerStats.OnDied += ShowDeathScreen;

            // Оновлюємо UI одразу при включенні на випадок, якщо значення вже існують
            UpdateLevelUI(playerStats.levl, playerStats.levl);
            UpdateExpUI(playerStats.currentExp, playerStats.expToNextLevel);
        }
        else
        {
            Debug.LogError("УВАГА: UIManager не може знайти об'єкт зі скриптом PlayerStats на сцені!");
        }
    }

    void OnDisable()
    {
        // Відписуємося безпечно
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateHealthUI;
            playerStats.OnStaminaChanged -= UpdateStaminaUI;
            playerStats.OnLevlChanged -= UpdateLevelUI;
            playerStats.OnExpChanged -= UpdateExpUI;
            playerStats.OnDied -= ShowDeathScreen;
        }
    }

    // Ця функція оновлює смужку ХП
    void UpdateHealthUI(float current, float max)
    {
        if (healthSlider != null) 
        {
            healthSlider.maxValue = max;    
            healthSlider.value = current;   
        }
        else
        {
            Debug.LogError("Увага: Не призначено Health Slider в UIManager! Перетягніть його в Інспекторі.");
        }
    }

    // Ця функція оновлює смужку стаміни
    void UpdateStaminaUI(float current, float max)
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = max;
            staminaSlider.value = current;
        }
        else
        {
            Debug.LogError("Увага: Не призначено Stamina Slider в UIManager! Перетягніть його в Інспекторі.");
        }
    }

    // Оновлення тексту рівня
    void UpdateLevelUI(int currentLevel, int maxLevel)
    {
        if (levelText != null)
        {
            levelText.text = "Lvl: " + currentLevel;
        }
    }

    // Оновлення тексту досвіду
    void UpdateExpUI(int currentExp, int expToNext)
    {
        if (expText != null)
        {
            expText.text = "EXP: " + currentExp + " / " + expToNext;
        }
    }

    void ShowDeathScreen()
    {
        Debug.Log("Показуємо екран Game Over!");
    }
}