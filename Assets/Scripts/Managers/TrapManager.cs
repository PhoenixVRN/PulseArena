using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Менеджер ловушек - спавнит черные дыры на арене
/// </summary>
public class TrapManager : MonoBehaviour
{
    [Header("Black Hole Settings")]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private int minBlackHoles = 1;
    [SerializeField] private int maxBlackHoles = 3;

    [Header("Spawn Settings")]
    [SerializeField] private float arenaRadius = 18f; // Чуть меньше границы арены
    [SerializeField] private float minDistanceFromCenter = 5f; // Минимальное расстояние от центра
    [SerializeField] private float minDistanceBetweenHoles = 4f; // Минимальное расстояние между дырами

    [Header("Size Randomization")]
    [SerializeField] private Vector2 sizeRange = new Vector2(0.5f, 2f); // Мин и макс размер
    [SerializeField] private bool randomizeOnStart = true;
    [SerializeField] private float respawnInterval = 15f; // Интервал пересоздания дыр (0 = без пересоздания)

    [Header("Advanced")]
    [SerializeField] private bool avoidPlayerSpawn = true;
    [SerializeField] private float playerSafeRadius = 8f;

    private List<GameObject> activeBlackHoles = new List<GameObject>();
    private float lastRespawnTime = 0f;

    void Start()
    {
        if (randomizeOnStart)
        {
            SpawnRandomBlackHoles();
        }
    }

    void Update()
    {
        // Пересоздание дыр через интервал
        if (respawnInterval > 0 && Time.time - lastRespawnTime > respawnInterval)
        {
            ClearBlackHoles();
            SpawnRandomBlackHoles();
            lastRespawnTime = Time.time;
            
            Debug.Log($"🔄 Черные дыры пересозданы! Новое расположение.");
        }
    }

    public void SpawnRandomBlackHoles()
    {
        int count = Random.Range(minBlackHoles, maxBlackHoles + 1);
        
        Debug.Log($"🕳️ Создаю {count} черных дыр на арене...");

        for (int i = 0; i < count; i++)
        {
            Vector2 position = GetRandomValidPosition();
            float size = Random.Range(sizeRange.x, sizeRange.y);
            
            SpawnBlackHole(position, size);
        }
    }

    Vector2 GetRandomValidPosition()
    {
        int maxAttempts = 30;
        Vector2 position = Vector2.zero;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Случайная позиция в кольце (не в центре, не у края)
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minDistanceFromCenter, arenaRadius);
            position = randomDirection * randomDistance;

            // Проверка 1: Не слишком близко к игроку
            if (avoidPlayerSpawn && player != null)
            {
                float distanceToPlayer = Vector2.Distance(position, player.transform.position);
                if (distanceToPlayer < playerSafeRadius)
                {
                    continue; // Слишком близко к игроку, пробуем снова
                }
            }

            // Проверка 2: Не слишком близко к другим дырам
            bool tooClose = false;
            foreach (GameObject hole in activeBlackHoles)
            {
                if (hole == null) continue;
                
                float distance = Vector2.Distance(position, hole.transform.position);
                if (distance < minDistanceBetweenHoles)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                return position; // Нашли подходящую позицию!
            }
        }

        // Если не нашли за 30 попыток - вернем хоть что-то
        Debug.LogWarning("⚠️ Не удалось найти идеальную позицию для дыры, использую последнюю попытку");
        return position;
    }

    void SpawnBlackHole(Vector2 position, float size)
    {
        if (blackHolePrefab == null)
        {
            Debug.LogError("TrapManager: Префаб черной дыры не назначен!");
            return;
        }

        GameObject hole = Instantiate(blackHolePrefab, position, Quaternion.identity);
        hole.transform.SetParent(transform); // Организуем иерархию
        hole.transform.localScale = Vector3.one * size;
        hole.tag = "BlackHole"; // Устанавливаем тег для обнаружения врагами
        
        // Настраиваем коллайдер под новый размер
        CircleCollider2D collider = hole.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            // Коллайдер должен занимать весь визуальный размер дыры
            // Базовый радиус = 0.5 (половина Circle Sprite), умножаем на размер
            collider.radius = 0.5f; // Это правильно - radius = 0.5 покрывает весь круг при scale = size
        }

        // Настраиваем компонент BlackHole
        BlackHole blackHoleScript = hole.GetComponent<BlackHole>();
        if (blackHoleScript != null)
        {
            // Радиус притяжения пропорционален размеру дыры
            blackHoleScript.SetPullRadius(size * 3f); // В 3 раза больше визуального размера
        }

        activeBlackHoles.Add(hole);
        
        Debug.Log($"✅ Черная дыра создана на позиции {position}, размер: {size:F2}");
    }

    public void ClearBlackHoles()
    {
        foreach (GameObject hole in activeBlackHoles)
        {
            if (hole != null)
            {
                Destroy(hole);
            }
        }
        
        activeBlackHoles.Clear();
        Debug.Log("🗑️ Все черные дыры удалены");
    }

    // Вызывается из других скриптов для создания дыры в конкретном месте
    public void SpawnBlackHoleAt(Vector2 position, float size = 1f)
    {
        SpawnBlackHole(position, size);
    }

    // Визуализация зоны спавна в редакторе
    void OnDrawGizmosSelected()
    {
        // Внешняя граница
        Gizmos.color = Color.yellow;
        DrawCircle(Vector2.zero, arenaRadius, 50);

        // Внутренняя граница (мертвая зона)
        Gizmos.color = Color.green;
        DrawCircle(Vector2.zero, minDistanceFromCenter, 30);

        // Безопасная зона игрока
        if (avoidPlayerSpawn)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Gizmos.color = new Color(0, 1f, 0, 0.3f);
                DrawCircle(player.transform.position, playerSafeRadius, 30);
            }
        }
    }

    void DrawCircle(Vector2 center, float radius, int segments)
    {
        float angle = 0f;
        Vector3 lastPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        
        for (int i = 1; i <= segments; i++)
        {
            angle = (float)i / segments * 2f * Mathf.PI;
            Vector3 newPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }
}
