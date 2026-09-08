using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Налаштування атаки")]
    public float damage = 20f;
    
    private EnemyFollow enemyMovement; // Посилання на наш скрипт руху

    void Start()
    {
        // Знаходимо скрипт руху на цьому ж клопі
        enemyMovement = GetComponent<EnemyFollow>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            
            if (playerStats != null)
            {
                // Наносимо шкоду
                playerStats.TakeDamage(damage);
                
                // Змушуємо ворога відійти назад після укусу!
                if (enemyMovement != null)
                {
                    enemyMovement.BounceBack();
                }
            }
        }
    }
}