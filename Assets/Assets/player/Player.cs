using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    public float speed = 5f; // Швидкість персонажа
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // GetAxisRaw робить рух різким, без інерції (ідеально для рогаликів)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Нормалізуємо вектор руху
        // Це потрібно, щоб при русі по діагоналі (одночасно вгору і вправо) 
        // персонаж не біг швидше, ніж по прямій.
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // Переміщуємо персонажа
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}