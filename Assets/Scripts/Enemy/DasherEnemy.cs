using UnityEngine;
using System.Collections;

/// <summary>
/// Враг с рывком - стоит на месте, затем резко врывается к игроку
/// </summary>
[RequireComponent(typeof(EnemyAI))]
public class DasherEnemy : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDistance = 8f;
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float chargeUpTime = 0.8f; // Время подготовки

    [Header("Visual")]
    [SerializeField] private Color chargeColor = Color.yellow;
    [SerializeField] private TrailRenderer trail;

    private EnemyAI enemyAI;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Transform player;

    private float lastDashTime = -999f;
    private bool isCharging = false;
    private bool isDashing = false;
    private Vector2 dashDirection;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Создаем trail если нет
        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
            trail.time = 0.3f;
            trail.startWidth = 0.5f;
            trail.endWidth = 0f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.startColor = Color.yellow;
            trail.endColor = new Color(1, 1, 0, 0);
            trail.emitting = false;
        }
    }

    void Update()
    {
        if (player == null || isCharging || isDashing) return;

        // Проверка возможности рывка
        if (Time.time - lastDashTime > dashCooldown)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            if (distanceToPlayer > 3f && distanceToPlayer < dashDistance + 2f)
            {
                StartCoroutine(ChargeAndDash());
            }
        }
    }

    IEnumerator ChargeAndDash()
    {
        isCharging = true;
        
        // Останавливаем обычное движение
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Визуальная подготовка
        float elapsed = 0f;
        while (elapsed < chargeUpTime)
        {
            if (spriteRenderer != null)
            {
                float t = (Mathf.Sin(elapsed * 15f) + 1f) * 0.5f;
                spriteRenderer.color = Color.Lerp(originalColor, chargeColor, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Возвращаем цвет
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // РЫВОК!
        if (player != null)
        {
            dashDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
            ExecuteDash();
        }

        isCharging = false;
    }

    void ExecuteDash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        if (rb != null)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }

        if (trail != null)
        {
            trail.emitting = true;
        }

        Debug.Log($"{gameObject.name}: РЫВОК к игроку!");

        // Останавливаем рывок через время
        Invoke(nameof(StopDash), dashDistance / dashSpeed);
    }

    void StopDash()
    {
        isDashing = false;

        if (rb != null)
        {
            rb.linearVelocity *= 0.3f; // Резкое торможение
        }

        if (trail != null)
        {
            trail.emitting = false;
        }

        // Возобновляем обычное движение
        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // При столкновении во время рывка - останавливаемся
        if (isDashing)
        {
            StopDash();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Радиус рывка
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dashDistance);

        // Направление рывка
        if (isCharging && player != null)
        {
            Gizmos.color = Color.red;
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction * dashDistance);
        }
    }
}
