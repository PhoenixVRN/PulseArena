using UnityEngine;

/// <summary>
/// Визуальный эффект импульса - расширяется и исчезает (2D версия)
/// </summary>
public class ImpulseEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float expandDuration = 0.3f;
    [SerializeField] private float maxScale = 10f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float startTime;
    private Vector3 initialScale;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        startTime = Time.time;
        initialScale = transform.localScale;

        // Получаем SpriteRenderer для изменения прозрачности
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogWarning("ImpulseEffect: SpriteRenderer не найден! Добавьте SpriteRenderer для визуала.");
        }
    }

    void Update()
    {
        float elapsed = Time.time - startTime;
        float progress = elapsed / expandDuration;

        if (progress >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        // Расширение (только в плоскости XY для 2D)
        float scaleMultiplier = scaleCurve.Evaluate(progress) * maxScale;
        transform.localScale = new Vector3(
            initialScale.x * scaleMultiplier,
            initialScale.y * scaleMultiplier,
            initialScale.z // Z не меняем
        );

        // Затухание прозрачности
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f - progress;
            spriteRenderer.color = color;
        }
    }
}