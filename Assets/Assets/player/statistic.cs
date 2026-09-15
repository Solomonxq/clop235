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
    public int expToNextLevel = 100;    
    public float expScalingMultiplier = 1.5f; 
    
    [Header("Хвилі (Wave)")]
    public int currentWave = 1; 

    // Ключі для PlayerPrefs
    private const string LevelSaveKey = "PlayerLevel";
    private const string ExpSaveKey = "PlayerExp";
    private const string WaveSaveKey = "PlayerWave";

    [Header("Витривалість (Stamina)")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f; 

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnStaminaChanged;
    public event Action<int, int> OnLevlChanged;
    public event Action<int, int> OnExpChanged; 
    public event Action<int> OnWaveChanged;
    public event Action OnDied;

    void Awake()
    {
        LoadData();
    }

    void OnEnable()
    {
        EnemyHealth.OnEnemyDied += HandleEnemyDeath;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= HandleEnemyDeath;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        RefreshUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        OnLevlChanged?.Invoke(levl, levl);
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
        OnWaveChanged?.Invoke(currentWave);
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
            OnDied?.Invoke();
            OnDead(); 
        }
    }

    private void HandleEnemyDeath(int enemyLevel)
    {
        int gainedExp = enemyLevel * 20;
        AddExp(gainedExp);
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

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
        SaveData();
        currentHealth = maxHealth;
        SceneManager.LoadScene("Startlocation");
    }

    private void LevelUp()
    {
        levl++;
        RecalculateExpToNextLevel();
        Debug.Log($"[LEVEL UP] Рівень {levl}! Потрібно XP: {expToNextLevel}");
        OnLevlChanged?.Invoke(levl, levl);
        SaveData();
    }

    private void RecalculateExpToNextLevel()
    {
        expToNextLevel = Mathf.RoundToInt(100f * Mathf.Pow(expScalingMultiplier, levl - 1));
    }

    public void SetWave(int newWave)
    {
        currentWave = newWave;
        OnWaveChanged?.Invoke(currentWave); // ДОДАНО: Сповіщаємо UI про зміну хвилі
        SaveData(); 
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
        PlayerPrefs.SetInt(WaveSaveKey, currentWave);
        PlayerPrefs.Save(); 
        
        Debug.Log($"[ЗБЕРЕЖЕНО] Рівень: {levl}, Досвід: {currentExp}, Хвиля: {currentWave}");
    }

    public void LoadData()
    {
        // PlayerPrefs.GetInt самостійно повертає значення за замовчуванням (1 або 0), якщо ключа ще немає
        levl = PlayerPrefs.GetInt(LevelSaveKey, 1);
        currentExp = PlayerPrefs.GetInt(ExpSaveKey, 0);
        currentWave = PlayerPrefs.GetInt(WaveSaveKey, 1);

        RecalculateExpToNextLevel();
        Debug.Log($"[ЗАВАНТАЖЕНО] Рівень: {levl}, Досвід: {currentExp}, Хвиля: {currentWave}, Поріг XP: {expToNextLevel}");
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LevelSaveKey);
        PlayerPrefs.DeleteKey(ExpSaveKey);
        PlayerPrefs.DeleteKey(WaveSaveKey);
        
        levl = 1;
        currentExp = 0;
        currentWave = 1;
        RecalculateExpToNextLevel();
        
        SaveData();
        RefreshUI();
    }
}