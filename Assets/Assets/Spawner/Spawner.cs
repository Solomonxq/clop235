using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    [Header("Посилання")]
    public PlayerStats playerStats;
    public GameObject enemyPrefab;

    [Header("Налаштування ліміту НА КАРТІ")]
    public int baseMaxActiveEnemies = 10;    
    public int activeEnemiesPer5Levels = 5;   

    [Header("Налаштування ЗАГАЛЬНОЇ кількості")]
    public int baseTotalEnemies = 20;        
    
    private int spawnedTotalCount = 0;        
    private int currentWaveTotalEnemies = 0;  
    private int enemiesKilledInCurrentWave = 0;

    [Header("Час спавну")]
    public float spawnInterval = 3f;
    private float spawnTimer;

    [Header("Точки спавну")]
    public Transform[] spawnPoints;          

    public event Action<int> OnWaveChanged;
    public event Action<int, int> OnWaveProgressChanged; 

    private bool waveEnded = false;

    void Awake()
    {
        FindActivePlayer();
    }

    void Start()
    {
        if (playerStats == null)
        {
            FindActivePlayer();
        }

        CalculateWaveTotal();

        if (playerStats != null)
        {
            playerStats.RefreshUI();
            OnWaveChanged?.Invoke(playerStats.currentWave);
        }
        
        OnWaveProgressChanged?.Invoke(enemiesKilledInCurrentWave, currentWaveTotalEnemies);
    }

    // Примусово шукаємо Гравця саме на СЦЕНІ, а не в папці Assets (Префаб)
    private void FindActivePlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerStats = playerObj.GetComponent<PlayerStats>();
        }
        else
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }

        if (playerStats == null)
        {
            Debug.LogError("[Spawner] Помилка! На сцені не знайдено об'єкт з PlayerStats або тегом 'Player'.");
        }
    }

    void OnEnable()
    {
        EnemyHealth.OnEnemyDied += HandleEnemyDeathForWave;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= HandleEnemyDeathForWave;
    }

    void CalculateWaveTotal()
    {
        int waveMultiplier = playerStats != null ? (playerStats.currentWave - 1) * 5 : 0;
        currentWaveTotalEnemies = baseTotalEnemies + waveMultiplier;
        enemiesKilledInCurrentWave = 0;
        spawnedTotalCount = 0;
    }

    void Update()
    {
        if (waveEnded) return; 

        if (spawnedTotalCount >= currentWaveTotalEnemies)
        {
            EndWave();
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnEnemy();
        }
    }

    void TrySpawnEnemy()
    { 
        if (playerStats == null || enemyPrefab == null) return;

        if (spawnedTotalCount >= currentWaveTotalEnemies)
        {
            EndWave();
            return;
        }

        int levelBlock = (playerStats.levl - 1) / 5;
        int currentMaxActiveEnemies = baseMaxActiveEnemies + levelBlock * activeEnemiesPer5Levels;

        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (activeEnemies.Length < currentMaxActiveEnemies)
        { 
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position;
        
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform randomPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            spawnPosition = randomPoint.position;
        }

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedTotalCount++;
    }

    void HandleEnemyDeathForWave(int enemyLevel)
    {
        if (waveEnded) return;

        enemiesKilledInCurrentWave++;
        if (enemiesKilledInCurrentWave > currentWaveTotalEnemies) 
            enemiesKilledInCurrentWave = currentWaveTotalEnemies;

        OnWaveProgressChanged?.Invoke(enemiesKilledInCurrentWave, currentWaveTotalEnemies);
    }

    void EndWave()
    {
        if (waveEnded) return;

        if (spawnedTotalCount >= currentWaveTotalEnemies)
        {
            GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (activeEnemies.Length == 0)
            {
                waveEnded = true;

                // Якщо посилання втратилося — знайдемо знову перед збереженням
                if (playerStats == null) FindActivePlayer();

                if (playerStats != null)
                {
                    // Оновлюємо хвилю та зберігаємо актуальні дані живого гравця
                    playerStats.SetWave(playerStats.currentWave + 1);
                    OnWaveChanged?.Invoke(playerStats.currentWave);
                }

                // Перехід на стартову локацію
                SceneManager.LoadScene("Startlocation");
            }
        }
    }
}