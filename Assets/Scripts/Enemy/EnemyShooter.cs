using UnityEngine;
using System.Collections;

/// <summary>
/// Враг-стрелок - может использовать импульс как игрок
/// Добавляется к EnemyAI как дополнительный компонент
/// </summary>
public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float impulseRadius = 4f;
    [SerializeField] private float impulseForce = 15f; // Слабее чем у игрока
    [SerializeField] private float impulseCooldown = 3f; // Дольше чем у игрока
    [SerializeField] private float shootRange = 8f; // Дистанция стрельбы

    [Header("AI Behavior")]
    [SerializeField] private float minShootDistance = 3f; // Минимальная дистанция (не стреляет вплотную)
    [SerializeField] private float maxShootDistance = 10f; // Максимальная дистанция
    [SerializeField] private float shootChance = 0.7f; // Шанс выстрела когда кулдаун готов (0-1)

    [Header("Visual")]
    [SerializeField] private GameObject enemyImpulsePrefab; // Префаб волны врага (красная)
    [SerializeField] private float chargeUpTime = 0.5f; // Время подготовки перед выстрелом
    [SerializeField] private Color chargeColor = Color.red; // Цвет при подготовке

    [Header("Advanced")]
    [SerializeField] private bool shootAtPlayer = true; // Стреляет в игрока
    [SerializeField] private bool avoidFriendlyFire = true; // Не стреляет если заденет союзников

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float lastShootTime = -999f;
    private bool isCharging = false;

    void Start()
    {
        // Найти игрока
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        if (player == null || isCharging) return;

        // Проверяем возможность выстрела
        if (CanShoot())
        {
            // Случайный шанс выстрела
            if (Random.value < shootChance * Time.deltaTime)
            {
                StartCoroutine(ChargeAndShoot());
            }
        }
    }

    bool CanShoot()
    {
        // Проверка кулдауна
        if (Time.time - lastShootTime < impulseCooldown)
        {
            return false;
        }

        // Проверка расстояния до игрока
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer < minShootDistance || distanceToPlayer > maxShootDistance)
        {
            return false;
        }

        // Проверка friendly fire
        if (avoidFriendlyFire && WouldHitAllies())
        {
            return false;
        }

        return true;
    }

    bool WouldHitAllies()
    {
        // Проверяем, есть ли союзники в радиусе
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, impulseRadius);
        
        foreach (Collider2D col in hitColliders)
        {
            // Если нашли другого врага в радиусе - не стреляем
            if (col.CompareTag("Enemy") && col.gameObject != gameObject)
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator ChargeAndShoot()
    {
        isCharging = true;

        // Визуальное предупреждение - мигание
        float elapsed = 0f;
        while (elapsed < chargeUpTime)
        {
            if (spriteRenderer != null)
            {
                // Мигаем между красным и оригинальным цветом
                float t = (Mathf.Sin(elapsed * 20f) + 1f) * 0.5f;
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

        // ВЫСТРЕЛ!
        Shoot();

        isCharging = false;
    }

    void Shoot()
    {
        Debug.Log($"🔴 {gameObject.name} стреляет импульсом!");

        lastShootTime = Time.time;

        // Визуальный эффект волны
        if (enemyImpulsePrefab != null)
        {
            GameObject impulseEffect = Instantiate(enemyImpulsePrefab, transform.position, Quaternion.identity);
            Destroy(impulseEffect, 1f);
        }

        // Физика - отбрасываем всё в радиусе (2D)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, impulseRadius);
        
        int targetsHit = 0;

        foreach (Collider2D col in hitColliders)
        {
            // Не толкаем себя
            if (col.gameObject == gameObject) continue;

            Rigidbody2D targetRb = col.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                // Направление от врага к цели
                Vector2 direction = ((Vector2)col.transform.position - (Vector2)transform.position).normalized;
                
                // Применяем силу
                targetRb.AddForce(direction * impulseForce, ForceMode2D.Impulse);

                // Визуальный эффект на цели
                if (col.CompareTag("Player"))
                {
                    targetsHit++;
                    Debug.Log($"🎯 Враг попал в игрока импульсом!");
                    StartCoroutine(FlashTarget(col.GetComponent<SpriteRenderer>(), Color.red));
                }
                else if (col.CompareTag("Enemy"))
                {
                    targetsHit++;
                    StartCoroutine(FlashTarget(col.GetComponent<SpriteRenderer>(), Color.yellow));
                }
            }
        }

        if (targetsHit > 0)
        {
            Debug.Log($"💥 Враг отбросил {targetsHit} целей!");
        }
    }

    IEnumerator FlashTarget(SpriteRenderer targetSprite, Color flashColor)
    {
        if (targetSprite == null) yield break;

        Color originalColor = targetSprite.color;
        targetSprite.color = flashColor;

        yield return new WaitForSeconds(0.1f);

        if (targetSprite != null)
        {
            targetSprite.color = originalColor;
        }
    }

    // Визуализация радиуса импульса в редакторе
    void OnDrawGizmosSelected()
    {
        // Радиус импульса (красный)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impulseRadius);

        // Дистанция стрельбы (желтый)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minShootDistance);
        Gizmos.DrawWireSphere(transform.position, maxShootDistance);
    }

    // Публичные методы для управления извне

    public void ForceShoot()
    {
        if (!isCharging)
        {
            StartCoroutine(ChargeAndShoot());
        }
    }

    public float GetCooldownProgress()
    {
        float timePassed = Time.time - lastShootTime;
        return Mathf.Clamp01(timePassed / impulseCooldown);
    }

    public bool IsReadyToShoot()
    {
        return Time.time - lastShootTime >= impulseCooldown;
    }
}
