using UnityEngine;

/// <summary>
/// Эффект волны импульса - расширяющееся кольцо (2D)
/// </summary>
public class ImpulseWaveEffect : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float expandSpeed = 30f;
    [SerializeField] private float maxRadius = 10f;
    [SerializeField] private float lifetime = 0.5f;
    [SerializeField] private Color waveColor = new Color(0, 1f, 1f, 1f); // Голубой

    private float currentRadius = 0f;
    private float startTime;
    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;

    void Start()
    {
        startTime = Time.time;

        // Используем LineRenderer для рисования кольца
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        SetupLineRenderer();
    }

    void SetupLineRenderer()
    {
        lineRenderer.positionCount = 50; // Количество точек в круге
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        
        // Цвет и материал
        lineRenderer.startColor = waveColor;
        lineRenderer.endColor = waveColor;
        
        // Материал - используем простой Sprites/Default
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.sortingLayerName = "Effects";
        lineRenderer.sortingOrder = 10;
    }

    void Update()
    {
        float elapsed = Time.time - startTime;
        float progress = elapsed / lifetime;

        if (progress >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        // Расширение кольца
        currentRadius = Mathf.Lerp(0f, maxRadius, progress);

        // Обновляем позиции точек LineRenderer
        UpdateCircle(currentRadius);

        // Затухание прозрачности
        Color fadedColor = waveColor;
        fadedColor.a = 1f - progress;
        lineRenderer.startColor = fadedColor;
        lineRenderer.endColor = fadedColor;

        // Уменьшение толщины линии
        float width = Mathf.Lerp(0.3f, 0.1f, progress);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    void UpdateCircle(float radius)
    {
        int segments = lineRenderer.positionCount;
        
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * 2f * Mathf.PI;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
