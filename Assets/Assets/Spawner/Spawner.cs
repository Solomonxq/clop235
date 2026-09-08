using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Посилання")]
    public PlayerStats playerStats;
    public GameObject enemyPrefab;

    [Header("Налаштування ліміту НА КАРТІ (збільшується кожні 5 левелів)")]
    public int baseMaxActiveEnemies = 10;     // Базовий ліміт на карті на 1-4 рівнях
    public int activeEnemiesPer5Levels = 5;   // Скільки додається до ліміту карти кожні 5 рівнів

    [Header("Налаштування ЗАГАЛЬНОЇ кількості (збільшується на 2 з кожним левелом)")]
    public int baseTotalEnemies = 20;         // Базова загальна кількість на 1 рівні
    
    private int spawnedTotalCount = 0;        // Скільки всього вже спавнер створив за забіг

    [Header("Час спавну")]
    public float spawnInterval = 3f;
    private float spawnTimer;

    [Header("Точки спавну (опціонально)")]
    public Transform[] spawnPoints;           // Якщо ви використовуєте масив точок

    void Start()
    {
        // Автоматично шукаємо гравця, якщо не вказано в інспекторі
        if (playerStats == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerStats = player.GetComponent<PlayerStats>();
            }
        }
    }

    void Update()
    {
        // Поточний ліміт за всю гру (збільшується на 2 з кожним новим левелом)
        int currentTotalLimit = baseTotalEnemies + (playerStats != null ? (playerStats.levl - 1) * 2 : 0);

        // Якщо вже спарнили загальний ліміт — зупиняємо спавн
        if (spawnedTotalCount >= currentTotalLimit) return;

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

        // 1. Поточний загальний ліміт (збільшується на 2 з кожним левелом)
        int currentTotalLimit = baseTotalEnemies + (playerStats.levl - 1) * 2;
        if (spawnedTotalCount >= currentTotalLimit) return;

        // 2. Ліміт ОДНОЧАСНО на карті (збільшується КОЖНІ 5 левелів)
        int levelBlock = (playerStats.levl - 1) / 5;
        int currentMaxActiveEnemies = baseMaxActiveEnemies + levelBlock * activeEnemiesPer5Levels;

        // Рахуємо скільки зараз клопів на карті (за тегом "Enemy")
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        // Якщо на карті менше активного ліміту — спавнимо нового
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
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPosition = randomPoint.position;
        }

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Збільшуємо лічильник створених за всю гру
        spawnedTotalCount++;
    }
}