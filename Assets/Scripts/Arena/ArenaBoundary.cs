using UnityEngine;

/// <summary>
/// Граница арены - уничтожает объекты, вышедшие за пределы (2D версия)
/// </summary>
public class ArenaBoundary : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float arenaRadius = 20f;
    [SerializeField] private Vector2 arenaCenter = Vector2.zero;

    void Update()
    {
        // Проверяем игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CheckBoundary(player);
        }

        // Проверяем врагов
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (CheckBoundary(enemy))
            {
                // Враг вышел за границу - уничтожаем
                Debug.Log($"💀 Враг {enemy.name} вышел за границу!");
                
                // Уведомляем GameManager
                GameManager gm = FindFirstObjectByType<GameManager>();
                if (gm != null)
                {
                    gm.OnEnemyDied();
                }
                
                Destroy(enemy);
            }
        }
    }

    bool CheckBoundary(GameObject obj)
    {
        float distance = Vector2.Distance((Vector2)obj.transform.position, arenaCenter);
        
        if (distance > arenaRadius)
        {
            if (obj.CompareTag("Player"))
            {
                // Игрок вышел за границу - Game Over
                Debug.LogWarning("💀 GAME OVER! Игрок вышел за границу!");
                // TODO: Вызвать Game Over
                return false; // Не уничтожаем игрока пока
            }
            
            return true; // Объект вышел за границу
        }

        return false;
    }

    // Визуализация границы в редакторе (2D - круг на плоскости XY)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        // Рисуем круг в 2D (на плоскости XY)
        int segments = 50;
        float angle = 0f;
        Vector3 lastPoint = arenaCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * arenaRadius;
        
        for (int i = 1; i <= segments; i++)
        {
            angle = (float)i / segments * 2f * Mathf.PI;
            Vector3 newPoint = arenaCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * arenaRadius;
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }
}