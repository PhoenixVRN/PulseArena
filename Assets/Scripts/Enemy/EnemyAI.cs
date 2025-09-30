using UnityEngine;

/// <summary>
/// AI врага - следует за игроком (2D версия)
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float pushResistance = 1f; // 1 = нормально, 2 = тяжелее (Heavy Orb)

    [Header("Knockback")]
    [SerializeField] private float knockbackRecoveryTime = 0.5f; // Время "оглушения" после импульса
    [SerializeField] private float knockbackThreshold = 2f; // Минимальная скорость для определения отброса

    [Header("References")]
    private Transform player;
    private Rigidbody2D rb;
    
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

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

        // Двигаемся к игроку через AddForce (учитывает физику!)
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;

        // Используем AddForce вместо MovePosition
        rb.AddForce(direction * moveSpeed, ForceMode2D.Force);

        // Ограничиваем максимальную скорость преследования
        float maxChaseSpeed = moveSpeed;
        if (rb.linearVelocity.magnitude > maxChaseSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxChaseSpeed;
        }
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

    // Визуализация состояния в редакторе
    void OnDrawGizmos()
    {
        if (isKnockedBack)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.6f);
        }
    }
}