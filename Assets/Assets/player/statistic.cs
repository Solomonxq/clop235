using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    [Header("Здоров'я (HP)")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Витривалість (Stamina)")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f; 

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnStaminaChanged;
    public event Action OnDied;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    void Update()
    {
        // Регенерація стаміни
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }

    public void TakeDamage(float amount)
    {
        // Перевіряємо лише чи гравець ще живий
        if (currentHealth <= 0) return;

        // Отримуємо шкоду
        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Перевірка на смерть
        if (currentHealth <= 0)
        {
            OnDied?.Invoke(); 
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            return true;
        }
        return false;
    }
}