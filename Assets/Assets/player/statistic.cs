using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Здоров'я (HP)")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Рівень та Досвід")]
    public int levl = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;     // Скільки треба досвіду для 1->2 рівня
    public float expScalingMultiplier = 1.5f; // На скільки збільшується вимога з кожним рівнем
    private const string LevelSaveKey = "PlayerLevel";
    private const string ExpSaveKey = "PlayerExp";

    [Header("Витривалість (Stamina)")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f; 

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnStaminaChanged;
    public event Action<int, int> OnLevlChanged;
    public event Action<int, int> OnExpChanged; // Поточний досвід, досвід до наступного рівня
    public event Action OnDied;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        
        LoadData();

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        OnLevlChanged?.Invoke(levl, levl);
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
    }

    void OnEnable()
    {
        EnemyHealth.OnEnemyDied += HandleEnemyDeath;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= HandleEnemyDeath;
    }

    void Update()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        if (currentHealth <= 0)
        {
            OnDead(); 
        }
    }

    // Метод спрацьовує, коли будь-який ворог помирає
    private void HandleEnemyDeath(int enemyLevel)
    {
        // Розраховуємо досвід: наприклад, базово 20 XP * рівень ворога
        int gainedExp = enemyLevel * 20;
        AddExp(gainedExp);
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

        // Перевірка на підвищення рівня (може підвищитися кілька разів, якщо дали забагато досвіду)
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        OnExpChanged?.Invoke(currentExp, expToNextLevel);
        SaveData();
    }

    public void OnDead()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void LevelUp()
    {
        levl++;
        
        // Збільшуємо кількість потрібного досвіду для наступного рівня за формулою
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expScalingMultiplier);

        Debug.Log("Вітаю! Гравець досяг " + levl + " рівня! Наступний рівень вимагає: " + expToNextLevel + " XP");

        OnLevlChanged?.Invoke(levl, levl);
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        
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

    public void SaveData()
    {
        PlayerPrefs.SetInt(LevelSaveKey, levl);
        PlayerPrefs.SetInt(ExpSaveKey, currentExp);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        levl = PlayerPrefs.GetInt(LevelSaveKey, 1);
        currentExp = PlayerPrefs.GetInt(ExpSaveKey, 0);

        // Перераховуємо вимоги до наступного рівня відповідно до поточного збереженого рівня
        expToNextLevel = 100;
        for (int i = 1; i < levl; i++)
        {
            expToNextLevel = Mathf.RoundToInt(expToNextLevel * expScalingMultiplier);
        }
    }

    public void ResetProgress()
    {
        levl = 1;
        currentExp = 0;
        expToNextLevel = 100;
        SaveData();
        OnLevlChanged?.Invoke(levl, levl);
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
    }
}