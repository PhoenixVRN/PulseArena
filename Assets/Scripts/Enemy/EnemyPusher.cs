using UnityEngine;

/// <summary>
/// Компонент для физического выталкивания игрока врагами
/// Добавляется ко всем типам врагов
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPusher : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float pushForce = 5f; // Сила выталкивания
    [SerializeField] private bool continuousPush = true; // Постоянно толкать при контакте

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Толкаем игрока при столкновении
        if (collision.gameObject.CompareTag("Player"))
        {
            PushPlayer(collision.gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Продолжаем толкать если враг касается игрока
        if (continuousPush && collision.gameObject.CompareTag("Player"))
        {
            PushPlayer(collision.gameObject);
        }
    }

    void PushPlayer(GameObject player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            // Направление от врага к игроку
            Vector2 pushDirection = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
            
            // Применяем силу к игроку
            playerRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);

            Debug.Log($"⚠️ {gameObject.name} толкает игрока!");
        }
    }
}
