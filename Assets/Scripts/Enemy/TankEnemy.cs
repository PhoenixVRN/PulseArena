using UnityEngine;

/// <summary>
/// Танк - медленный, устойчивый, требует много импульсов
/// </summary>
[RequireComponent(typeof(EnemyAI))]
public class TankEnemy : MonoBehaviour
{
    [Header("Tank Settings")]
    [SerializeField] private int hitsToDestroy = 5; // Сколько импульсов до смерти
    [SerializeField] private float armorReduction = 0.5f; // Уменьшение силы отброса
    [SerializeField] private bool showHealthBar = true;

    [Header("Visual")]
    [SerializeField] private Color damageColor = Color.white;
    [SerializeField] private GameObject healthBarPrefab;

    private int currentHits = 0;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private GameObject healthBarInstance;
    private Transform healthBarFill;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Увеличиваем массу (тяжелый)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.mass = 3f;
        }

        // Увеличиваем размер
        transform.localScale = Vector3.one * 1.5f;

        // Создаем health bar
        if (showHealthBar)
        {
            CreateHealthBar();
        }
    }

    void CreateHealthBar()
    {
        // Простой health bar из двух спрайтов
        healthBarInstance = new GameObject("HealthBar");
        healthBarInstance.transform.SetParent(transform);
        healthBarInstance.transform.localPosition = new Vector3(0, 0.8f, 0);
        healthBarInstance.transform.localScale = Vector3.one;

        // Фон (красный)
        GameObject background = new GameObject("Background");
        background.transform.SetParent(healthBarInstance.transform);
        background.transform.localPosition = Vector3.zero;
        SpriteRenderer bgSprite = background.AddComponent<SpriteRenderer>();
        bgSprite.sprite = CreateQuadSprite();
        bgSprite.color = Color.red;
        bgSprite.sortingOrder = 10;
        background.transform.localScale = new Vector3(1f, 0.1f, 1);

        // Заполнение (зеленый)
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(healthBarInstance.transform);
        fill.transform.localPosition = Vector3.zero;
        SpriteRenderer fillSprite = fill.AddComponent<SpriteRenderer>();
        fillSprite.sprite = CreateQuadSprite();
        fillSprite.color = Color.green;
        fillSprite.sortingOrder = 11;
        fill.transform.localScale = new Vector3(1f, 0.1f, 1);
        
        healthBarFill = fill.transform;
    }

    Sprite CreateQuadSprite()
    {
        // Создаем простой белый спрайт
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        // Обновляем health bar
        if (healthBarFill != null)
        {
            float healthPercent = 1f - (float)currentHits / hitsToDestroy;
            healthBarFill.localScale = new Vector3(healthPercent, 0.1f, 1);
            
            // Меняем цвет
            SpriteRenderer fillSprite = healthBarFill.GetComponent<SpriteRenderer>();
            if (fillSprite != null)
            {
                fillSprite.color = Color.Lerp(Color.red, Color.green, healthPercent);
            }
        }
    }

    public void TakeHit(float impulseForceMagnitude)
    {
        currentHits++;

        Debug.Log($"{gameObject.name}: Получил удар {currentHits}/{hitsToDestroy}");

        // Визуальный эффект
        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashDamage());
        }

        // Проверка смерти
        if (currentHits >= hitsToDestroy)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashDamage()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    void Die()
    {
        Debug.Log($"💀 {gameObject.name}: Уничтожен!");

        // Уведомляем GameManager
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.OnEnemyDied();
        }

        Destroy(gameObject);
    }

    // Модификация силы отброса
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Если столкнулись с импульсом игрока
        if (collision.gameObject.CompareTag("Player"))
        {
            // Уменьшаем отброс через Rigidbody
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity *= armorReduction;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Показываем что враг тяжелый
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
