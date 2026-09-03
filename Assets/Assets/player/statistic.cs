using UnityEngine;
using System; // Обов'язково додаємо це для використання Action (івентів)

public class PlayerStats : MonoBehaviour
{
    [Header("Здоров'я (HP)")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Витривалість (Stamina)")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f; 

    // Створюємо івенти (радіо-канали), на які зможуть підписатися інші скрипти
    public event Action<float, float> OnHealthChanged;   // Передає поточне і максимальне ХП
    public event Action<float, float> OnStaminaChanged;  // Передає поточну і максимальну стаміну
    public event Action OnDied;                          // Нічого не передає, просто сповіщає про смерть

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        
        // Викликаємо івенти на старті, щоб UI одразу показав повні смужки
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    void Update()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
            
            // Сповіщаємо всіх слухачів, що стаміна відновилася
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        
        // Сповіщаємо всіх слухачів (наприклад, UI або скрипт звуку), що ХП змінилося
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            OnDied?.Invoke(); // Сповіщаємо про смерть
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            
            // Сповіщаємо про витрату стаміни
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            return true;
        }
        return false;
    }
}