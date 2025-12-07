using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Manages enemy, item, and boss spawning for Level 2 (Jungle)
// Enemies spawn continuously with increasing rate
// Items spawn every 20 seconds randomly
// Boss spawns at 4 minutes (1 minute before game end)
public class Level2Manager : MonoBehaviour
{
    [Header("Enemy Spawning")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private float initialEnemySpawnInterval = 3f; // Start with spawn every 3 seconds
    [SerializeField] private float minEnemySpawnInterval = 0.5f; // Minimum interval (max spawn rate)
    [SerializeField] private int maxEnemiesOnScreen = 30;

    [Header("Item Spawning")]
    [SerializeField] private GameObject[] itemPrefabs; // Should include: Star, SpeedBoost, HealthPack, DamageBoost
    [SerializeField] private Transform[] itemSpawnPoints;
    [SerializeField] private float itemSpawnInterval = 20f; // Every 20 seconds
    [SerializeField] private int maxItemsOnScreen = 10;

    [Header("Boss Spawning")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform[] bossSpawnPoints;
    [SerializeField] private float bossSpawnTime = 240f; // 4 minutes = 240 seconds (1 min before 5 min end)

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 50f; // Radius for random spawns if no spawn points
    [SerializeField] private LayerMask navMeshLayer;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> activeItems = new List<GameObject>();
    private float lastEnemySpawnTime = 0f;
    private float lastItemSpawnTime = 0f;
    private float gameStartTime = 0f;
    private bool bossSpawned = false;
    private GameObject currentBoss = null;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            gameStartTime = Time.time;
            lastEnemySpawnTime = Time.time;
            lastItemSpawnTime = Time.time;
        }
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        float elapsedTime = Time.time - gameStartTime;

        // Spawn enemies with increasing frequency
        UpdateEnemySpawning(elapsedTime);

        // Spawn items every 20 seconds
        UpdateItemSpawning();

        // Spawn boss at 4 minutes
        UpdateBossSpawning(elapsedTime);

        // Clean up destroyed objects from lists
        activeEnemies.RemoveAll(enemy => enemy == null);
        activeItems.RemoveAll(item => item == null);
    }

    void UpdateEnemySpawning(float elapsedTime)
    {
        // Calculate current spawn interval (decreases over time = spawns more frequently)
        // Linear interpolation from initial to minimum interval over the global game duration
        float gameDuration = (GameManager.Instance != null)
            ? GameManager.Instance.GetGameDuration()
            : 180f;
        float progress = Mathf.Clamp01(elapsedTime / gameDuration);
        float currentSpawnInterval = Mathf.Lerp(initialEnemySpawnInterval, minEnemySpawnInterval, progress);

        // Spawn enemy if enough time has passed and not at max
        if (Time.time - lastEnemySpawnTime >= currentSpawnInterval && activeEnemies.Count < maxEnemiesOnScreen)
        {
            SpawnEnemy();
            lastEnemySpawnTime = Time.time;
        }
    }

    void UpdateItemSpawning()
    {
        if (Time.time - lastItemSpawnTime >= itemSpawnInterval && activeItems.Count < maxItemsOnScreen)
        {
            SpawnRandomItem();
            lastItemSpawnTime = Time.time;
        }
    }

    void UpdateBossSpawning(float elapsedTime)
    {
        if (!bossSpawned && elapsedTime >= bossSpawnTime && bossPrefab != null)
        {
            SpawnBoss();
            bossSpawned = true;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        // Pick random enemy prefab
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Vector3 spawnPosition;
        Quaternion spawnRotation = Quaternion.identity;

        // Get spawn position
        if (enemySpawnPoints != null && enemySpawnPoints.Length > 0)
        {
            Transform spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
            spawnPosition = spawnPoint.position;
            spawnRotation = spawnPoint.rotation;
        }
        else
        {
            // Random position on NavMesh if no spawn points
            spawnPosition = GetRandomNavMeshPosition();
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        activeEnemies.Add(enemy);
    }

    void SpawnRandomItem()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0) return;

        // Random item type: Star, SpeedBoost, HealthPack, or DamageBoost
        GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        Vector3 spawnPosition;
        Quaternion spawnRotation = Quaternion.identity;

        // Get spawn position
        if (itemSpawnPoints != null && itemSpawnPoints.Length > 0)
        {
            Transform spawnPoint = itemSpawnPoints[Random.Range(0, itemSpawnPoints.Length)];
            spawnPosition = spawnPoint.position;
            spawnRotation = spawnPoint.rotation;
        }
        else
        {
            // Random position on NavMesh if no spawn points
            spawnPosition = GetRandomNavMeshPosition();
        }

        GameObject item = Instantiate(itemPrefab, spawnPosition, spawnRotation);
        activeItems.Add(item);
    }

    void SpawnBoss()
    {
        if (bossPrefab == null) return;

        Vector3 spawnPosition;
        Quaternion spawnRotation = Quaternion.identity;

        // Get spawn position
        if (bossSpawnPoints != null && bossSpawnPoints.Length > 0)
        {
            Transform spawnPoint = bossSpawnPoints[Random.Range(0, bossSpawnPoints.Length)];
            spawnPosition = spawnPoint.position;
            spawnRotation = spawnPoint.rotation;
        }
        else
        {
            // Random position on NavMesh if no spawn points
            spawnPosition = GetRandomNavMeshPosition();
        }

        currentBoss = Instantiate(bossPrefab, spawnPosition, spawnRotation);
        Debug.Log("Boss spawned at " + spawnPosition);
    }

    Vector3 GetRandomNavMeshPosition()
    {
        // Try to find a random position on the NavMesh
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;

        // Get player position as center point, or use origin
        Vector3 centerPoint = Vector3.zero;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            centerPoint = player.transform.position;
        }

        randomDirection += centerPoint;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // Fallback to center point if NavMesh sample fails
        return centerPoint;
    }

    public void ClearAllSpawns()
    {
        // Clear all enemies
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();

        // Clear all items
        foreach (GameObject item in activeItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        activeItems.Clear();

        // Clear boss
        if (currentBoss != null)
        {
            Destroy(currentBoss);
            currentBoss = null;
        }

        bossSpawned = false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw enemy spawn points
        if (enemySpawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in enemySpawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 1f);
                }
            }
        }

        // Draw item spawn points
        if (itemSpawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform spawnPoint in itemSpawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                }
            }
        }

        // Draw boss spawn points
        if (bossSpawnPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform spawnPoint in bossSpawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 2f);
                }
            }
        }
    }
}

