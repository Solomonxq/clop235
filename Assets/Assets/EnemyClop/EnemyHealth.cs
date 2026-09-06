using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Візуал та позиція шкоди")]
    public GameObject damageTextPrefab;
    public float textOffsetY = 0.3f; 
    public float textOffsetX = 0.1f; 
    public float textOffsetZ = -0.5f; 

    [Header("Налаштування випадіння луту")]
    [SerializeField] private GameObject itemPrefab; // Префаб предмету, який випадає
    [Range(0f, 100f)]
    [SerializeField] private float dropChance = 50f; // Шанс випадіння у відсотках

    void Start()
    {
        currentHealth = maxHealth;
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
        // 1. Перевіряємо відстань до кола ураження
        float distanceToAttack = Vector2.Distance(transform.position, attackPoint);

        if (distanceToAttack <= attackRadius)
        {
            // 2. Напрямок удару та напрямок від гравця до ворога
            Vector2 attackDir = (attackPoint - playerPosition).normalized;
            Vector2 enemyDir = ((Vector2)transform.position - playerPosition).normalized;

            // Скалярний добуток: > 0 означає, що ворог знаходиться спереду напрямку удару
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
        Debug.Log(gameObject.name + " отримав удар! Залишилось здоров'я: " + currentHealth);

        if (damageTextPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(
                transform.position.x + Random.Range(-textOffsetX, textOffsetX),
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
        Debug.Log(gameObject.name + " помер!");

        // Викликаємо випадіння луту перед тим, як вимкнути ворога
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

        // Повністю видаляємо об'єкт ворога зі сцени
        Destroy(gameObject);
    }
    private void DropItem()
    {
        if (itemPrefab == null) return;

        // Генеруємо випадкове число від 0 до 100 для перевірки шансу
        float roll = Random.Range(0f, 100f);

        if (roll <= dropChance)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
    }
}