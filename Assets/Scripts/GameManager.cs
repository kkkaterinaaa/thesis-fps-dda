using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Enemies")]
    public GameObject[] enemyPrefabs;
    public int maxEnemies = 15;
    public float spawnInterval = 3f;
    private float spawnTimer = 0f;

    [Header("Enemy Stats")]
    public EnemyStats lightEnemyStats;   // Подключаем EnemyStats для лёгкого врага
    public EnemyStats mediumEnemyStats;  // Подключаем EnemyStats для среднего врага
    public EnemyStats heavyEnemyStats;   // Подключаем EnemyStats для тяжёлого врага

    [Header("Spawn Settings")]
    public float spawnRadius = 12f;
    public float minDistanceBetweenEnemies = 2f;
    public int maxTries = 12;

    [Header("References")]
    public Transform player;

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            TrySpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void TrySpawnEnemy()
    {
        // limit total enemies in scene
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (currentEnemies >= maxEnemies)
            return;

        // find free position on NavMesh
        Vector3 pos;
        if (!TryGetFreeSpawnPosition(out pos))
            return;

        // choose random enemy type
        int i = Random.Range(0, enemyPrefabs.Length);

        // spawn
        GameObject enemy = Instantiate(enemyPrefabs[i], pos, Quaternion.identity);

        // Apply stats to the new enemy
        var controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.stats = GetRandomEnemyStats(); // Присваиваем EnemyStats
            controller.ApplyStats();  // Применяем статистику к врагу
        }
    }

    EnemyStats GetRandomEnemyStats()
    {
        // Возвращаем случайный EnemyStats
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0: return lightEnemyStats;   // Light
            case 1: return mediumEnemyStats;  // Medium
            case 2: return heavyEnemyStats;   // Heavy
            default: return lightEnemyStats;  // Default to light
        }
    }

    bool TryGetFreeSpawnPosition(out Vector3 result)
    {
        for (int attempt = 0; attempt < maxTries; attempt++)
        {
            Vector3 randomPos = player.position +
                                new Vector3(
                                    Random.Range(-spawnRadius, spawnRadius),
                                    0,
                                    Random.Range(-spawnRadius, spawnRadius)
                                );

            NavMeshHit hit;
            if (!NavMesh.SamplePosition(randomPos, out hit, 3f, NavMesh.AllAreas))
                continue;

            // ensure space between enemies
            if (!Physics.CheckSphere(hit.position, minDistanceBetweenEnemies, LayerMask.GetMask("Enemy")))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}