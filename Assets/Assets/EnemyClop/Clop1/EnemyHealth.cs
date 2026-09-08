using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Характеристики та рівень")]
    public int enemyLevel = 1;              
    public float baseHealth = 100f;         
    public float healthMultiplier = 1.2f;   
    public float damageMultiplier = 1.15f;  
    
    [HideInInspector] public float currentMaxHealth;
    private float currentHealth;

    [Header("Візуал та позиція шкоди")]
    public GameObject damageTextPrefab;
    public float textOffsetY = 0.3f; 
    public float textOffsetX = 0.1f; 
    public float textOffsetZ = -0.5f; 

    [Header("Налаштування випадіння луту")]
    [SerializeField] private GameObject itemPrefab; 
    [Range(0f, 100f)]
    [SerializeField] private float dropChance = 50f; 

    // --- ІВЕНТ СМЕРТІ ВОРОГА (передає рівень ворога) ---
    public static event Action<int> OnEnemyDied;

    void Start()
    {
        CalculateStats();
        currentHealth = currentMaxHealth;
    }

    public void InitializeLevel(int level)
    {
        enemyLevel = Mathf.Max(1, level);
        CalculateStats();
        currentHealth = currentMaxHealth;
    }

    private void CalculateStats()
    {
        currentMaxHealth = baseHealth * Mathf.Pow(healthMultiplier, enemyLevel - 1);
    }

    public float GetScaledDamage(float baseDamage)
    {
        return baseDamage * Mathf.Pow(damageMultiplier, enemyLevel - 1);
    }

    void OnEnable()
    {
        PlayerAttack.OnPlayerAttacked += CheckIfHit;
    }

    void OnDisable()
    {
        PlayerAttack.OnPlayerAttacked -= CheckIfHit;
    }

    private void CheckIfHit(Vector2 playerPosition, Vector2 attackPoint, float attackRadius, float damage)
    {
        float distanceToAttack = Vector2.Distance(transform.position, attackPoint);

        if (distanceToAttack <= attackRadius)
        {
            Vector2 attackDir = (attackPoint - playerPosition).normalized;
            Vector2 enemyDir = ((Vector2)transform.position - playerPosition).normalized;

            if (Vector2.Dot(enemyDir, attackDir) > 0)
            {
                TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;

        if (damageTextPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(
                transform.position.x + UnityEngine.Random.Range(-textOffsetX, textOffsetX),
                transform.position.y + textOffsetY,
                transform.position.z + textOffsetZ
            );

            GameObject textObj = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);
            
            DamageText damageText = textObj.GetComponent<DamageText>();
            if (damageText != null)
            {
                damageText.Setup(amount);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Викликаємо івент смерті і передаємо рівень цього ворога
        OnEnemyDied?.Invoke(enemyLevel);

        DropItem();

        EnemyFollow followScript = GetComponent<EnemyFollow>();
        if (followScript != null) followScript.enabled = false;

        EnemyAttack attackScript = GetComponent<EnemyAttack>();
        if (attackScript != null) attackScript.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        Destroy(gameObject);
    }

    private void DropItem()
    {
        if (itemPrefab == null) return;

        float roll = UnityEngine.Random.Range(0f, 100f);

        if (roll <= dropChance)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
    }
}