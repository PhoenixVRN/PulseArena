using UnityEngine;

/// <summary>
/// Черная дыра - уничтожает всё что попадает внутрь
/// </summary>
public class BlackHole : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float pullForce = 5f; // Сила притяжения
    [SerializeField] private float pullRadius = 3f; // Радиус притяжения (больше чем коллайдер)
    [SerializeField] private bool visualizeRadius = true;

    [Header("Visual")]
    [SerializeField] private float rotationSpeed = 30f; // Скорость вращения
    [SerializeField] private ParticleSystem particles; // Опциональные частицы

    private CircleCollider2D triggerCollider;

    void Start()
    {
        triggerCollider = GetComponent<CircleCollider2D>();
        
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }

        // Создаем частицы если их нет
        if (particles == null)
        {
            CreateParticles();
        }
    }

    void Update()
    {
        // Вращение дыры для визуала
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        // Притягиваем объекты в радиусе
        PullNearbyObjects();
    }

    void PullNearbyObjects()
    {
        // Находим все объекты в радиусе притяжения
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, pullRadius);

        foreach (Collider2D col in nearbyObjects)
        {
            // Пропускаем триггеры и саму дыру
            if (col.isTrigger || col.gameObject == gameObject) continue;

            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Направление к центру дыры
                Vector2 direction = ((Vector2)transform.position - rb.position).normalized;
                float distance = Vector2.Distance(transform.position, rb.position);

                // Сила притяжения убывает с расстоянием
                float pullStrength = pullForce * (1f - distance / pullRadius);
                
                rb.AddForce(direction * pullStrength, ForceMode2D.Force);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Уничтожаем врагов
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"🕳️ Враг {other.gameObject.name} упал в черную дыру!");
            
            // Уведомляем GameManager
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.OnEnemyDied();
            }

            Destroy(other.gameObject);
        }
        // Уничтожаем игрока (Game Over)
        else if (other.CompareTag("Player"))
        {
            Debug.LogError("💀💀💀 GAME OVER! Игрок упал в черную дыру! 💀💀💀");
            
            // TODO: Вызвать Game Over экран
            
            // Пока просто уничтожаем игрока (перезапуск сцены)
            Time.timeScale = 0.5f; // Замедляем время для драматичности
            Destroy(other.gameObject, 0.5f); // Уничтожаем через полсекунды
            
            // Можно добавить перезагрузку сцены
            // Invoke("RestartGame", 1f);
        }
    }

    // Метод для установки радиуса притяжения (вызывается из TrapManager)
    public void SetPullRadius(float radius)
    {
        pullRadius = radius;
    }

    // Метод для перезагрузки игры (пока закомментирован)
    /*
    void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
    */

    void CreateParticles()
    {
        // Создаем простые частицы программно
        GameObject particlesObj = new GameObject("Particles");
        particlesObj.transform.SetParent(transform);
        particlesObj.transform.localPosition = Vector3.zero;
        
        particles = particlesObj.AddComponent<ParticleSystem>();
        
        var main = particles.main;
        main.startLifetime = 1f;
        main.startSpeed = 1f;
        main.startSize = 0.2f;
        main.startColor = new Color(0.5f, 0f, 0.8f, 0.5f); // Фиолетовый
        main.maxParticles = 50;
        
        var emission = particles.emission;
        emission.rateOverTime = 20;
        
        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = pullRadius * 0.8f;
    }

    // Визуализация радиуса притяжения в редакторе
    void OnDrawGizmos()
    {
        if (!visualizeRadius) return;

        // Радиус притяжения (желтый)
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, pullRadius);

        // Радиус уничтожения (красный)
        if (triggerCollider != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, triggerCollider.radius);
        }
    }
}
