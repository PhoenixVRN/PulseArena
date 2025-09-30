using UnityEngine;

/// <summary>
/// AI врага - следует за игроком (2D версия)
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float pushResistance = 1f; // 1 = нормально, 2 = тяжелее (Heavy Orb)

    [Header("References")]
    private Transform player;
    private Rigidbody2D rb;

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

    void FixedUpdate()
    {
        if (player == null) return;

        // Двигаемся к игроку (2D - только X и Y)
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;

        // Используем MovePosition для плавного движения
        Vector2 targetPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
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
}