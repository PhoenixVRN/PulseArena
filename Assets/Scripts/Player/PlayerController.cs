using UnityEngine;

/// <summary>
/// Управление игроком - движение и импульс (2D версия)
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxSpeed = 15f;

    [Header("Impulse")]
    [SerializeField] private float impulseRadius = 5f;
    [SerializeField] private float impulseForce = 20f;
    [SerializeField] private float impulseCooldown = 1.5f;

    [Header("Visual")]
    [SerializeField] private GameObject impulsePrefab; // Префаб визуального эффекта волны
    [SerializeField] private float screenShakeDuration = 0.2f;
    [SerializeField] private float screenShakeIntensity = 0.3f;

    private Rigidbody2D rb;
    private float lastImpulseTime = -999f;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("PlayerController: Rigidbody2D не найден! Добавьте Rigidbody2D к игроку.");
        }
    }

    void Update()
    {
        // Ввод движения
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // Импульс
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            TryUseImpulse();
        }
    }

    void FixedUpdate()
    {
        // Движение
        if (moveInput.magnitude > 0.1f)
        {
            rb.AddForce(moveInput * moveSpeed, ForceMode2D.Force);

            // Ограничение скорости
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }

    void TryUseImpulse()
    {
        // Проверка кулдауна
        if (Time.time - lastImpulseTime < impulseCooldown)
        {
            float remaining = impulseCooldown - (Time.time - lastImpulseTime);
            Debug.Log($"⏳ Импульс на перезарядке! Осталось: {remaining:F1}с");
            return;
        }

        UseImpulse();
        lastImpulseTime = Time.time;
    }

    void UseImpulse()
    {
        Debug.Log("💥 ИМПУЛЬС!");

        // Визуальный эффект волны
        if (impulsePrefab != null)
        {
            GameObject impulseEffect = Instantiate(impulsePrefab, transform.position, Quaternion.identity);
            Destroy(impulseEffect, 1f); // Уничтожить через 1 секунду
        }

        // Screen shake эффект
        CameraShake();

        // Физика - отбрасываем всё в радиусе (2D)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, impulseRadius);
        
        int enemiesHit = 0;

        foreach (Collider2D col in hitColliders)
        {
            // Не толкаем себя
            if (col.gameObject == gameObject) continue;

            Rigidbody2D targetRb = col.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                // Направление от игрока к цели
                Vector2 direction = ((Vector2)col.transform.position - (Vector2)transform.position).normalized;
                
                // Применяем силу
                targetRb.AddForce(direction * impulseForce, ForceMode2D.Impulse);
                Debug.Log($"namr {col.gameObject}/ {direction * impulseForce}");
                // Визуальный эффект на враге
                if (col.CompareTag("Enemy"))
                {
                    enemiesHit++;
                    StartCoroutine(FlashEnemy(col.GetComponent<SpriteRenderer>()));

                    // Если это танк - наносим урон
                    TankEnemy tank = col.GetComponent<TankEnemy>();
                    if (tank != null)
                    {
                        tank.TakeHit(impulseForce);
                    }
                }

                Debug.Log($"🎯 Отброшен: {col.gameObject.name}");
            }
        }

        if (enemiesHit > 0)
        {
            Debug.Log($"💪 Попадание! Врагов отброшено: {enemiesHit}");
        }
        else
        {
            Debug.Log("❌ Промах! Нет врагов в радиусе.");
        }
    }

    // Эффект вспышки на враге при попадании
    System.Collections.IEnumerator FlashEnemy(SpriteRenderer enemySprite)
    {
        if (enemySprite == null) yield break;

        Color originalColor = enemySprite.color;
        enemySprite.color = Color.white; // Вспышка белым

        yield return new WaitForSeconds(0.1f);

        if (enemySprite != null)
        {
            enemySprite.color = originalColor;
        }
    }

    // Screen shake эффект
    void CameraShake()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraShakeEffect shakeEffect = mainCam.GetComponent<CameraShakeEffect>();
            if (shakeEffect == null)
            {
                shakeEffect = mainCam.gameObject.AddComponent<CameraShakeEffect>();
            }
            shakeEffect.Shake(screenShakeDuration, screenShakeIntensity);
        }
    }

    // Визуализация радиуса импульса в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, impulseRadius);
    }

    // Геттер для UI (кулдаун)
    public float GetImpulseCooldownProgress()
    {
        float timePassed = Time.time - lastImpulseTime;
        return Mathf.Clamp01(timePassed / impulseCooldown);
    }
}