using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Управление игрой - спавн волн с разными типами врагов, счёт, состояние
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject basicEnemyPrefab;
    [SerializeField] private GameObject shooterEnemyPrefab;
    [SerializeField] private GameObject fastEnemyPrefab;
    [SerializeField] private GameObject dasherEnemyPrefab;
    [SerializeField] private GameObject teleporterEnemyPrefab;
    [SerializeField] private GameObject tankEnemyPrefab;

    [Header("Wave Settings")]
    [SerializeField] private Transform arenaCenter;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float timeBetweenWaves = 3f;

    [Header("Wave Progression")]
    [SerializeField] private int shooterUnlockWave = 3; // С какой волны появляются Shooter
    [SerializeField] private int fastUnlockWave = 4; // Fast Enemy
    [SerializeField] private int dasherUnlockWave = 5; // Dasher
    [SerializeField] private int teleporterUnlockWave = 6; // Teleporter
    [SerializeField] private int tankUnlockWave = 7; // Tank

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
        
        Debug.Log($"🌊 ВОЛНА {currentWave}!");

        // Определяем состав волны
        List<GameObject> waveComposition = GetWaveComposition(currentWave);

        // Спавним врагов
        foreach (GameObject enemyPrefab in waveComposition)
        {
            if (enemyPrefab != null)
            {
                SpawnEnemy(enemyPrefab);
            }
        }

        enemiesAlive = waveComposition.Count;
        Debug.Log($"Врагов в волне: {enemiesAlive}");
    }

    List<GameObject> GetWaveComposition(int wave)
    {
        List<GameObject> enemies = new List<GameObject>();

        // ВОЛНА 1-2: Только Basic
        if (wave <= 2)
        {
            for (int i = 0; i < 3 + wave; i++)
            {
                enemies.Add(basicEnemyPrefab);
            }
        }
        // ВОЛНА 3-4: Basic + Shooter
        else if (wave <= 4)
        {
            for (int i = 0; i < 2; i++)
            {
                enemies.Add(basicEnemyPrefab);
            }
            enemies.Add(shooterEnemyPrefab);
            
            if (wave == 4)
            {
                enemies.Add(shooterEnemyPrefab);
            }
        }
        // ВОЛНА 5-6: Basic + Shooter + Fast
        else if (wave <= 6)
        {
            enemies.Add(basicEnemyPrefab);
            enemies.Add(shooterEnemyPrefab);
            
            for (int i = 0; i < 2; i++)
            {
                enemies.Add(fastEnemyPrefab);
            }
            
            if (wave == 6)
            {
                enemies.Add(dasherEnemyPrefab);
            }
        }
        // ВОЛНА 7-8: Разнообразие + Tank
        else if (wave <= 8)
        {
            enemies.Add(tankEnemyPrefab);
            enemies.Add(basicEnemyPrefab);
            enemies.Add(basicEnemyPrefab);
            enemies.Add(shooterEnemyPrefab);
            enemies.Add(fastEnemyPrefab);
            
            if (wave == 8)
            {
                enemies.Add(dasherEnemyPrefab);
                enemies.Add(teleporterEnemyPrefab);
            }
        }
        // ВОЛНА 9+: Экстрим
        else
        {
            // Танки
            int tankCount = Mathf.Min((wave - 7) / 2, 3);
            for (int i = 0; i < tankCount; i++)
            {
                enemies.Add(tankEnemyPrefab);
            }

            // Shooter
            for (int i = 0; i < 2; i++)
            {
                enemies.Add(shooterEnemyPrefab);
            }

            // Dasher
            for (int i = 0; i < 2; i++)
            {
                enemies.Add(dasherEnemyPrefab);
            }

            // Teleporter
            enemies.Add(teleporterEnemyPrefab);
            enemies.Add(teleporterEnemyPrefab);

            // Fast
            for (int i = 0; i < 3; i++)
            {
                enemies.Add(fastEnemyPrefab);
            }
        }

        return enemies;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("GameManager: Префаб врага не назначен!");
            return;
        }

        // Случайная позиция по кругу вокруг центра арены (2D)
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = (Vector2)arenaCenter.position + randomCircle;
        spawnPosition.z = 0; // Обнуляем Z для 2D

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.name = $"{enemyPrefab.name}_Wave{currentWave}";

        Debug.Log($"✅ Создан: {enemy.name} на позиции {spawnPosition}");
    }

    // Вызывается, когда враг умирает
    public void OnEnemyDied()
    {
        enemiesAlive--;
        score += 10;
        
        Debug.Log($"💀 Враг уничтожен! Осталось: {enemiesAlive}, Счёт: {score}");
    }

    // Геттеры для UI
    public int GetCurrentWave() => currentWave;
    public int GetScore() => score;
    public int GetEnemiesAlive() => enemiesAlive;

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