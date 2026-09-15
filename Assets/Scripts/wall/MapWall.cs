using UnityEngine;

// Додаємо очікування колайдера, щоб Unity автоматично його перевіряла
[RequireComponent(typeof(Collider2D))]
public class MapWall : MonoBehaviour
{
    private Collider2D wallCollider;

    public Collider2D WallCollider
    {
        get
        {
            if (wallCollider == null)
                wallCollider = GetComponent<Collider2D>();
            return wallCollider;
        }
    }

    // Візуалізація стін у редакторі (жовта рамка), щоб їх було легко бачити
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}