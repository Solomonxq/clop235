using UnityEngine;
using System.Collections; // Обов'язково для таймерів відходу

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rb;
    private Vector2 movement;
    public float speed = 5f;
    public float stoppingDistance = 0.5f; 
    
    [Header("Налаштування відходу після атаки")]
    public float retreatSpeed = 4f;       // З якою швидкістю ворог відходить назад
    public float retreatDuration = 0.5f;  // Скільки часу він йде назад (в секундах)
    private bool isRetreating = false;    // Чи відходить він зараз

    private bool isTouchingPlayer = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Якщо ворог зараз відходить назад — ігноруємо звичайний рух до гравця
        if (isRetreating) return; 

        if (player != null)
        {
            if (isTouchingPlayer)
            {
                movement = Vector2.zero;
                return; 
            }

            Vector3 direction = player.position - transform.position;
            float distance = direction.magnitude;

            if (distance > stoppingDistance)
            {
                direction.Normalize();
                movement = direction;
            }
            else
            {
                movement = Vector2.zero;
            }
        }
    }

    private void FixedUpdate()
    {
        // Якщо відходить назад — фізичний рух контролюється корутиною, тому тут нічого не робимо
        if (isRetreating) return;

        if (isTouchingPlayer)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) isTouchingPlayer = true; 
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) isTouchingPlayer = false; 
    }

    // --- НОВИЙ МЕТОД: Цю функцію буде викликати скрипт атаки ---
    public void BounceBack()
    {
        if (!isRetreating)
        {
            StartCoroutine(RetreatRoutine());
        }
    }

   private IEnumerator RetreatRoutine()
    {
        isRetreating = true;
        isTouchingPlayer = false; // Примусово скидаємо дотик

        // Визначаємо різницю координат між клопом і гравцем
        Vector2 diff = transform.position - player.position;
        Vector2 directionAway;

        // Перевіряємо, з якого боку клоп ближче: по горизонталі (X) чи по вертикалі (Y)
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            // Якщо клоп знаходиться збоку, відкидаємо його ТІЛЬКИ вліво або вправо
            directionAway = new Vector2(Mathf.Sign(diff.x), 0); 
        }
        else
        {
            // Якщо клоп знаходиться зверху/знизу, відкидаємо його ТІЛЬКИ вгору або вниз
            directionAway = new Vector2(0, Mathf.Sign(diff.y)); 
        }

        // Штовхаємо клопа рівно по прямій
        rb.linearVelocity = directionAway * retreatSpeed;

        // Чекаємо вказаний час
        yield return new WaitForSeconds(retreatDuration);

        // Зупиняємо його і дозволяємо знову полювати
        rb.linearVelocity = Vector2.zero;
        isRetreating = false;
    }
}