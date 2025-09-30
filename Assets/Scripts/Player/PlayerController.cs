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
            Debug.Log($"Импульс на перезарядке! Осталось: {impulseCooldown - (Time.time - lastImpulseTime):F1}с");
            return;
        }

        UseImpulse();
        lastImpulseTime = Time.time;
    }

    void UseImpulse()
    {
        Debug.Log("💥 ИМПУЛЬС!");

        // Визуальный эффект
        if (impulsePrefab != null)
        {
            GameObject impulseEffect = Instantiate(impulsePrefab, transform.position, Quaternion.identity);
            Destroy(impulseEffect, 1f); // Уничтожить через 1 секунду
        }

        // Физика - отбрасываем всё в радиусе (2D)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, impulseRadius);
        
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

                Debug.Log($"Отброшен: {col.gameObject.name}");
            }
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