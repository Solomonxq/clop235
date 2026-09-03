using UnityEngine;
using UnityEngine.UI; // Обов'язково додаємо це, щоб працювати з UI елементами (Slider)

public class UIManager : MonoBehaviour
{
    public PlayerStats playerStats; 
    
    [Header("Смужки UI")]
    public Slider healthSlider;
    public Slider staminaSlider;

    void OnEnable()
    {
        // Підписуємося на події
        playerStats.OnHealthChanged += UpdateHealthUI;
        playerStats.OnStaminaChanged += UpdateStaminaUI; // Додали підписку на стаміну
        playerStats.OnDied += ShowDeathScreen;
    }

    void OnDisable()
    {
        // Відписуємося
        playerStats.OnHealthChanged -= UpdateHealthUI;
        playerStats.OnStaminaChanged -= UpdateStaminaUI;
        playerStats.OnDied -= ShowDeathScreen;
    }

    // Ця функція оновлює смужку ХП
    void UpdateHealthUI(float current, float max)
    {
        healthSlider.maxValue = max;    // Встановлюємо максимум на смужці (100)
        healthSlider.value = current;   // Встановлюємо поточне заповнення
    }

    // Ця функція оновлює смужку стаміни
    void UpdateStaminaUI(float current, float max)
    {
        staminaSlider.maxValue = max;
        staminaSlider.value = current;
    }

    void ShowDeathScreen()
    {
        Debug.Log("Показуємо екран Game Over!");
    }
}