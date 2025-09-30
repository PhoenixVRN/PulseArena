using UnityEngine;

/// <summary>
/// Быстрый враг - маленький, быстрый, хаотичный
/// </summary>
[RequireComponent(typeof(EnemyAI))]
public class FastEnemy : MonoBehaviour
{
    [Header("Fast Enemy Settings")]
    [SerializeField] private float zigzagFrequency = 2f; // Как часто меняет направление
    [SerializeField] private float zigzagAmplitude = 2f; // Насколько сильно отклоняется
    [SerializeField] private float burstSpeedMultiplier = 2f; // Множитель при рывке
    [SerializeField] private float burstDuration = 0.3f; // Длительность рывка
    [SerializeField] private float burstCooldown = 2f; // Кулдаун рывка

    private EnemyAI enemyAI;
    private Rigidbody2D rb;
    private float lastBurstTime = -999f;
    private bool isBursting = false;
    private float burstTimer = 0f;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();

        // Быстрые враги легче
        if (rb != null)
        {
            rb.mass = 0.5f;
        }
    }

    void Update()
    {
        // Рывок
        if (isBursting)
        {
            burstTimer -= Time.deltaTime;
            if (burstTimer <= 0f)
            {
                isBursting = false;
            }
        }
        else
        {
            // Проверка возможности рывка
            if (Time.time - lastBurstTime > burstCooldown && Random.value < 0.05f)
            {
                StartBurst();
            }
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Зигзаг-движение
        if (!isBursting)
        {
            Vector2 perpendicular = new Vector2(-rb.linearVelocity.y, rb.linearVelocity.x).normalized;
            float zigzag = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
            rb.AddForce(perpendicular * zigzag, ForceMode2D.Force);
        }
    }

    void StartBurst()
    {
        isBursting = true;
        burstTimer = burstDuration;
        lastBurstTime = Time.time;

        // Резкое ускорение в текущем направлении
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            rb.linearVelocity *= burstSpeedMultiplier;
        }

        Debug.Log($"{gameObject.name}: РЫВОК!");
    }

    void OnDrawGizmos()
    {
        if (isBursting)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.8f);
        }
    }
}
