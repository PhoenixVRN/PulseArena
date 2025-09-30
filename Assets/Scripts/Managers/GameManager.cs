using UnityEngine;
using System.Collections;

/// <summary>
/// Управление игрой - спавн волн, счёт, состояние (2D версия)
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform arenaCenter;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private int initialEnemyCount = 3;
    [SerializeField] private float timeBetweenWaves = 3f;

    [Header("Difficulty")]
    [SerializeField] private float enemyIncreasePerWave = 1f; // Сколько врагов добавлять каждую волну
    [SerializeField] private float speedIncreasePerWave = 0.1f; // Ускорение врагов

    private int currentWave = 0;
    private int enemiesAlive = 0;
    private int score = 0;
    private bool gameActive = false;

    void Start()
    {
        // Если arenaCenter не задан, используем (0,0)
        if (arenaCenter == null)
        {
            GameObject centerObj = new GameObject("ArenaCenter");
            arenaCenter = centerObj.transform;
            arenaCenter.position = Vector2.zero;
        }

        StartGame();
    }

    void Update()
    {
        // Проверяем, остались ли враги
        if (gameActive && enemiesAlive <= 0)
        {
            // Волна завершена
            StartCoroutine(StartNextWave());
        }
    }

    void StartGame()
    {
        gameActive = true;
        currentWave = 0;
        score = 0;
        
        Debug.Log("🎮 ИГРА НАЧАЛАСЬ!");
        SpawnWave();
    }

    IEnumerator StartNextWave()
    {
        gameActive = false; // Пауза между волнами
        
        Debug.Log($"✅ Волна {currentWave} завершена! Счёт: {score}");
        Debug.Log($"⏳ Следующая волна через {timeBetweenWaves}с...");
        
        yield return new WaitForSeconds(timeBetweenWaves);
        
        SpawnWave();
        gameActive = true;
    }

    void SpawnWave()
    {
        currentWave++;
        
        // Рассчитываем количество врагов
        int enemyCount = Mathf.RoundToInt(initialEnemyCount + (currentWave - 1) * enemyIncreasePerWave);
        
        Debug.Log($"🌊 ВОЛНА {currentWave}! Врагов: {enemyCount}");

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }

        enemiesAlive = enemyCount;
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("GameManager: Префаб врага не назначен!");
            return;
        }

        // Случайная позиция по кругу вокруг центра арены (2D)
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = (Vector2)arenaCenter.position + randomCircle;
        spawnPosition.z = 0; // Обнуляем Z для 2D

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Увеличиваем скорость врагов с каждой волной
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            // TODO: Добавить публичный метод для изменения скорости
            // enemyAI.SetSpeed(baseSpeed + currentWave * speedIncreasePerWave);
        }

        // Подписываемся на смерть врага (пока вручную, позже через событие)
        enemy.name = $"Enemy_{currentWave}_{enemiesAlive}";
    }

    // Вызывается, когда враг умирает
    public void OnEnemyDied()
    {
        enemiesAlive--;
        score += 10;
        
        Debug.Log($"💀 Враг уничтожен! Осталось: {enemiesAlive}, Счёт: {score}");
    }

    // Визуализация зоны спавна (2D круг)
    void OnDrawGizmosSelected()
    {
        if (arenaCenter == null) return;
        
        Gizmos.color = Color.yellow;
        
        // Рисуем круг в 2D
        int segments = 30;
        float angle = 0f;
        Vector3 lastPoint = (Vector2)arenaCenter.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        
        for (int i = 1; i <= segments; i++)
        {
            angle = (float)i / segments * 2f * Mathf.PI;
            Vector3 newPoint = (Vector2)arenaCenter.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }
}