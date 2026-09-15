using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Мертва зона (Dead Zone)")]
    public Vector2 deadZoneSize = new Vector2(6f, 4f);

    private Vector2 minPosition;
    private Vector2 maxPosition;
    private bool hasBounds = false;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        CalculateBoundsFromWalls();
    }

    // Автоматично шукає всі об'єкти зі скриптом MapWall і рахує загальну границю
    public void CalculateBoundsFromWalls()
    {
        MapWall[] walls = FindObjectsByType<MapWall>(FindObjectsSortMode.None);

        if (walls.Length == 0)
        {
            hasBounds = false;
            return;
        }

        // Початкові крайні точки
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        // Знаходимо найвіддаленіші краї серед усіх стін на сцені
        foreach (MapWall wall in walls)
        {
            Bounds b = wall.WallCollider.bounds;
            if (b.min.x < minX) minX = b.min.x;
            if (b.min.y < minY) minY = b.min.y;
            if (b.max.x > maxX) maxX = b.max.x;
            if (b.max.y > maxY) maxY = b.max.y;
        }

        // Враховуємо ортографічний розмір камери, щоб вона не показувала «за межами»
        float camVertExtent = cam.orthographicSize;
        float camHorzExtent = cam.orthographicSize * cam.aspect;

        minPosition = new Vector2(minX + camHorzExtent, minY + camVertExtent);
        maxPosition = new Vector2(maxX - camHorzExtent, maxY - camVertExtent);

        // Перевірка на випадок, якщо кімната менша за сама камеру
        if (minPosition.x > maxPosition.x)
        {
            float centerX = (minX + maxX) / 2f;
            minPosition.x = centerX;
            maxPosition.x = centerX;
        }
        if (minPosition.y > maxPosition.y)
        {
            float centerY = (minY + maxY) / 2f;
            minPosition.y = centerY;
            maxPosition.y = centerY;
        }

        hasBounds = true;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 currentCamPos = transform.position;

        // Мертва зона (Dead Zone)
        float deltaX = target.position.x - currentCamPos.x;
        float deltaY = target.position.y - currentCamPos.y;

        if (Mathf.Abs(deltaX) > deadZoneSize.x / 2f)
        {
            currentCamPos.x = target.position.x - (Mathf.Sign(deltaX) * (deadZoneSize.x / 2f));
        }

        if (Mathf.Abs(deltaY) > deadZoneSize.y / 2f)
        {
            currentCamPos.y = target.position.y - (Mathf.Sign(deltaY) * (deadZoneSize.y / 2f));
        }

        // Обмежуємо координати камери, якщо стіни знайдені
        if (hasBounds)
        {
            currentCamPos.x = Mathf.Clamp(currentCamPos.x, minPosition.x, maxPosition.x);
            currentCamPos.y = Mathf.Clamp(currentCamPos.y, minPosition.y, maxPosition.y);
        }

        transform.position = currentCamPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneSize.x, deadZoneSize.y, 0));
    }
}