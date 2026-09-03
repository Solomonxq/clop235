using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Мертва зона (Dead Zone)")]
    [Tooltip("Розмір зони по X та Y, у якій гравець може рухатися без руху камери")]
    public Vector2 deadZoneSize = new Vector2(3f, 2f);

    [Header("Границі карти")]
    public Vector2 minPosition;
    public Vector2 maxPosition;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 currentCamPos = transform.position;

        // Обчислюємо різницю між гравцем і камерою
        float deltaX = target.position.x - currentCamPos.x;
        float deltaY = target.position.y - currentCamPos.y;

        // Перевіряємо, чи вийшов гравець за межі 'мертвої зони' по X
        if (Mathf.Abs(deltaX) > deadZoneSize.x / 2f)
        {
            if (deltaX > 0)
                currentCamPos.x = target.position.x - (deadZoneSize.x / 2f);
            else
                currentCamPos.x = target.position.x + (deadZoneSize.x / 2f);
        }

        // Перевіряємо, чи вийшов гравець за межі 'мертвої зони' по Y
        if (Mathf.Abs(deltaY) > deadZoneSize.y / 2f)
        {
            if (deltaY > 0)
                currentCamPos.y = target.position.y - (deadZoneSize.y / 2f);
            else
                currentCamPos.y = target.position.y + (deadZoneSize.y / 2f);
        }

        // Обмежуємо камеру рамками карти (Min/Max)
        currentCamPos.x = Mathf.Clamp(currentCamPos.x, minPosition.x, maxPosition.x);
        currentCamPos.y = Mathf.Clamp(currentCamPos.y, minPosition.y, maxPosition.y);

        // Присвоюємо оновлену позицію
        transform.position = currentCamPos;
    }

    // Візуалізація мертвої зони в редакторі Unity (червоний прямокутник)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneSize.x, deadZoneSize.y, 0));
    }
}