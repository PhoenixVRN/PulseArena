using UnityEngine;

/// <summary>
/// AI врага - следует за игроком, избегает черные дыры (2D версия)
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float pushResistance = 1f; // 1 = нормально, 2 = тяжелее (Heavy Orb)

    [Header("Knockback")]
    [SerializeField] private float knockbackRecoveryTime = 0.5f; // Время "оглушения" после импульса
    [SerializeField] private float knockbackThreshold = 2f; // Минимальная скорость для определения отброса

    [Header("Black Hole Avoidance")]
    [SerializeField] private float blackHoleDetectionRange = 6f; // Радиус обнаружения дыр
    [SerializeField] private float blackHoleSafeDistance = 2f; // Безопасное расстояние от дыры
    [SerializeField] private float randomWalkDuration = 3f; // Как долго идти в случайную точку
    [SerializeField] private float pathCheckInterval = 0.5f; // Как часто проверять путь

    [Header("References")]
    private Transform player;
    private Rigidbody2D rb;
    
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    // Обход дыр
    private bool isAvoidingBlackHole = false;
    private Vector2 randomTargetPosition;
    private float avoidanceTimer = 0f;
    private float lastPathCheckTime = 0f;

    void Start()
    {
        // Найти игрока
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("EnemyAI: Игрок не найден! Добавьте тег 'Player' к игроку.");
        }

        rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            // Увеличиваем массу для тяжёлых врагов
            rb.mass *= pushResistance;
        }
    }

    void Update()
    {
        // Таймер восстановления после отброса
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
        }

        // Таймер обхода дыры
        if (isAvoidingBlackHole)
        {
            avoidanceTimer -= Time.deltaTime;
            if (avoidanceTimer <= 0f)
            {
                isAvoidingBlackHole = false;
                Debug.Log($"{gameObject.name}: Закончил обход дыры, возвращаюсь к преследованию");
            }
        }

        // Периодическая проверка пути (не каждый кадр, для оптимизации)
        if (Time.time - lastPathCheckTime > pathCheckInterval)
        {
            CheckPathToPlayer();
            lastPathCheckTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        // Проверяем, не отброшен ли враг
        if (rb.linearVelocity.magnitude > knockbackThreshold)
        {
            // Враг летит после импульса - не управляем им
            isKnockedBack = true;
            knockbackTimer = knockbackRecoveryTime;
            return;
        }

        // Если враг оглушён - не двигаемся
        if (isKnockedBack)
        {
            return;
        }

        // Определяем целевую позицию
        Vector2 targetPosition;
        
        if (isAvoidingBlackHole)
        {
            // Идём в случайную точку обхода
            targetPosition = randomTargetPosition;
        }
        else
        {
            // Идём к игроку
            targetPosition = player.position;
        }

        // Двигаемся к целевой позиции через AddForce (учитывает физику!)
        Vector2 direction = (targetPosition - rb.position).normalized;

        // Используем AddForce вместо MovePosition
        rb.AddForce(direction * moveSpeed, ForceMode2D.Force);

        // Ограничиваем максимальную скорость преследования
        float maxChaseSpeed = moveSpeed;
        if (rb.linearVelocity.magnitude > maxChaseSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxChaseSpeed;
        }
    }

    void CheckPathToPlayer()
    {
        if (player == null || isAvoidingBlackHole) return;

        // Находим все черные дыры рядом
        GameObject[] blackHoles = GameObject.FindGameObjectsWithTag("BlackHole");
        
        Vector2 directionToPlayer = ((Vector2)player.position - rb.position).normalized;
        float distanceToPlayer = Vector2.Distance(rb.position, player.position);

        foreach (GameObject hole in blackHoles)
        {
            if (hole == null) continue;

            Vector2 holePosition = hole.transform.position;
            float holeScale = hole.transform.localScale.x; // Размер дыры
            float holeDangerRadius = holeScale * 0.5f + blackHoleSafeDistance;

            // Проверка 1: Дыра слишком близко?
            float distanceToHole = Vector2.Distance(rb.position, holePosition);
            if (distanceToHole < holeDangerRadius)
            {
                StartAvoidance(holePosition);
                Debug.Log($"{gameObject.name}: Слишком близко к дыре! Начинаю обход.");
                return;
            }

            // Проверка 2: Дыра на пути к игроку?
            if (IsBlackHoleInPath(holePosition, holeDangerRadius, directionToPlayer, distanceToPlayer))
            {
                StartAvoidance(holePosition);
                Debug.Log($"{gameObject.name}: Дыра на пути к игроку! Начинаю обход.");
                return;
            }
        }
    }

    bool IsBlackHoleInPath(Vector2 holePosition, float holeDangerRadius, Vector2 directionToPlayer, float distanceToPlayer)
    {
        // Проверяем, находится ли дыра на прямой линии к игроку

        // Расстояние от дыры до линии движения (перпендикуляр)
        Vector2 toHole = holePosition - rb.position;
        float projectionLength = Vector2.Dot(toHole, directionToPlayer);

        // Дыра позади или дальше игрока - игнорируем
        if (projectionLength < 0 || projectionLength > distanceToPlayer)
        {
            return false;
        }

        // Точка на линии, ближайшая к дыре
        Vector2 closestPointOnPath = rb.position + directionToPlayer * projectionLength;
        float distanceFromPath = Vector2.Distance(holePosition, closestPointOnPath);

        // Если дыра близко к линии движения - она на пути!
        return distanceFromPath < holeDangerRadius;
    }

    void StartAvoidance(Vector2 holePosition)
    {
        isAvoidingBlackHole = true;
        avoidanceTimer = randomWalkDuration;

        // Генерируем случайную точку для обхода
        // Направление ОТ дыры
        Vector2 awayFromHole = (rb.position - holePosition).normalized;
        
        // Добавляем случайный угол отклонения (влево или вправо)
        float randomAngle = Random.Range(-90f, 90f);
        Vector2 randomDirection = Rotate(awayFromHole, randomAngle);

        // Случайное расстояние для точки обхода
        float randomDistance = Random.Range(3f, 6f);
        randomTargetPosition = rb.position + randomDirection * randomDistance;

        Debug.Log($"{gameObject.name}: Иду в обход к точке {randomTargetPosition}");
    }

    Vector2 Rotate(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        return new Vector2(
            cos * v.x - sin * v.y,
            sin * v.x + cos * v.y
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Если враг коснулся игрока - урон (пока просто лог)
        if (other.CompareTag("Player"))
        {
            Debug.Log("⚠️ Враг коснулся игрока!");
            // TODO: Система урона
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Альтернативный метод для физических коллайдеров
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("⚠️ Враг столкнулся с игроком!");
            // TODO: Система урона
        }
    }

    // Визуализация состояния в редакторе
    void OnDrawGizmos()
    {
        if (rb == null) return;

        // Желтый круг - оглушение после отброса
        if (isKnockedBack)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.6f);
        }

        // Оранжевая линия - путь к игроку
        if (player != null && !isAvoidingBlackHole)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }

        // Красная линия - путь обхода
        if (isAvoidingBlackHole)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, randomTargetPosition);
            Gizmos.DrawWireSphere(randomTargetPosition, 0.5f);
        }

        // Радиус обнаружения дыр
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, blackHoleDetectionRange);
    }
}