using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Додано для перевірки сцен

public class UIManager : MonoBehaviour
{
    public PlayerStats playerStats; 

    [Header("Налаштування Сцени")]
    public string arenaSceneName = "Arena"; // Точна назва вашої сцени Арени

    [Header("Елементи UI Арени")]
    public GameObject hotbar;               // Посилання на об'єкт Хотбару
    public Slider healthSlider;             // Смужка HP
    public Slider staminaSlider;            // Смужка Stamina

    [Header("UI Рівня та Досвіду")]
    public TextMeshProUGUI levelText;      
    public TextMeshProUGUI expText;        

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
        }

        if (playerStats != null)
        {
            playerStats.OnHealthChanged += UpdateHealthUI;
            playerStats.OnStaminaChanged += UpdateStaminaUI;
            playerStats.OnLevlChanged += UpdateLevelUI;
            playerStats.OnExpChanged += UpdateExpUI;
            playerStats.OnDied += ShowDeathScreen;

            UpdateLevelUI(playerStats.levl, playerStats.levl);
            UpdateExpUI(playerStats.currentExp, playerStats.expToNextLevel);
        }

        CheckSceneVisibility();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateHealthUI;
            playerStats.OnStaminaChanged -= UpdateStaminaUI;
            playerStats.OnLevlChanged -= UpdateLevelUI;
            playerStats.OnExpChanged -= UpdateExpUI;
            playerStats.OnDied -= ShowDeathScreen;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneVisibility();
    }

    private void CheckSceneVisibility()
    {
        bool isArena = SceneManager.GetActiveScene().name == arenaSceneName;

        // Вмикаємо HP, Stamina та Хотбар лише на Арені
        if (hotbar != null) hotbar.SetActive(isArena);
        if (healthSlider != null) healthSlider.gameObject.SetActive(isArena);
        if (staminaSlider != null) staminaSlider.gameObject.SetActive(isArena);

        // Поради щодо Рівня та Досвіду (залиште коментар за потреби):
        // if (levelText != null) levelText.gameObject.SetActive(isArena);
        // if (expText != null) expText.gameObject.SetActive(isArena);
    }

    void UpdateHealthUI(float current, float max)
    {
        if (healthSlider != null) 
        {
            healthSlider.maxValue = max;    
            healthSlider.value = current;   
        }
    }

    void UpdateStaminaUI(float current, float max)
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = max;
            staminaSlider.value = current;
        }
    }

    void UpdateLevelUI(int currentLevel, int maxLevel)
    {
        if (levelText != null)
        {
            levelText.text = "Lvl: " + currentLevel;
        }
    }

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