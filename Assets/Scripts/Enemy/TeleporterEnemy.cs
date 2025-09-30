using UnityEngine;
using System.Collections;

/// <summary>
/// Враг-телепортер - телепортируется вокруг игрока
/// </summary>
[RequireComponent(typeof(EnemyAI))]
public class TeleporterEnemy : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private float teleportCooldown = 4f;
    [SerializeField] private float minTeleportDistance = 3f;
    [SerializeField] private float maxTeleportDistance = 8f;
    [SerializeField] private float fadeTime = 0.3f; // Время исчезновения/появления

    [Header("Visual")]
    [SerializeField] private GameObject teleportEffectPrefab;
    [SerializeField] private Color fadeColor = new Color(0.5f, 0f, 0.8f, 1f); // Фиолетовый

    private EnemyAI enemyAI;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Transform player;

    private float lastTeleportTime = -999f;
    private bool isTeleporting = false;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
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
    }

    void Update()
    {
        if (player == null || isTeleporting) return;

        // Телепортируемся если далеко или застряли
        if (Time.time - lastTeleportTime > teleportCooldown)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // Условия для телепорта
            bool tooFar = distanceToPlayer > maxTeleportDistance + 3f;
            bool shouldTeleport = Random.value < 0.3f * Time.deltaTime; // Случайный шанс
            
            if (tooFar || shouldTeleport)
            {
                StartCoroutine(TeleportSequence());
            }
        }
    }

    IEnumerator TeleportSequence()
    {
        isTeleporting = true;
        lastTeleportTime = Time.time;

        // Останавливаем движение
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Эффект исчезновения
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        }

        // Fade out
        yield return StartCoroutine(FadeOut());

        // Телепортация
        Vector2 newPosition = GetRandomTeleportPosition();
        transform.position = newPosition;

        Debug.Log($"{gameObject.name}: Телепортация в {newPosition}");

        // Эффект появления
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        }

        // Fade in
        yield return StartCoroutine(FadeIn());

        // Возобновляем движение
        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }

        isTeleporting = false;
    }

    Vector2 GetRandomTeleportPosition()
    {
        if (player == null)
        {
            return transform.position;
        }

        // Случайная позиция вокруг игрока
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minTeleportDistance, maxTeleportDistance);
        Vector2 targetPosition = (Vector2)player.position + randomDirection * randomDistance;

        return targetPosition;
    }

    IEnumerator FadeOut()
    {
        if (spriteRenderer == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            
            // Меняем цвет и прозрачность
            Color currentColor = Color.Lerp(originalColor, fadeColor, t);
            currentColor.a = 1f - t;
            spriteRenderer.color = currentColor;

            // Уменьшаем масштаб
            transform.localScale = Vector3.one * (1f - t * 0.5f);

            yield return null;
        }

        spriteRenderer.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
    }

    IEnumerator FadeIn()
    {
        if (spriteRenderer == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            
            // Восстанавливаем цвет и прозрачность
            Color currentColor = Color.Lerp(fadeColor, originalColor, t);
            currentColor.a = t;
            spriteRenderer.color = currentColor;

            // Восстанавливаем масштаб
            transform.localScale = Vector3.one * (0.5f + t * 0.5f);

            yield return null;
        }

        spriteRenderer.color = originalColor;
        transform.localScale = Vector3.one;
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Радиус телепортации
        Gizmos.color = new Color(0.5f, 0f, 0.8f, 0.3f);
        Gizmos.DrawWireSphere(player.position, minTeleportDistance);
        Gizmos.DrawWireSphere(player.position, maxTeleportDistance);
    }
}
