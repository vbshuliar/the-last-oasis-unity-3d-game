using UnityEngine;

// spawns the boss after a delay and ensures the position offsets from center
public class BossSpawner : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private float spawnDelaySeconds = 90f;

    [Header("Spawn Area")]
    [SerializeField] private Transform spawnCenter;
    [SerializeField] private float minSpawnDistance = 35f;
    [SerializeField] private float maxSpawnDistance = 90f;
    [SerializeField] private int maxPositionAttempts = 12;

    private bool bossSpawned = false;
    private float elapsedTime = 0f;

    // prepares tracking when the spawner becomes active
    void OnEnable()
    {
        ResetSpawner();
    }

    // resets tracking when the spawner is disabled
    void OnDisable()
    {
        ResetSpawner();
    }

    // counts up time and spawns the boss once the delay elapses
    void Update()
    {
        if (bossSpawned || GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= spawnDelaySeconds)
        {
            SpawnBoss();
        }
    }

    // instantiates the boss prefab at a valid navmesh position
    void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("BossSpawner: Boss prefab not assigned.");
            return;
        }

        if (!SpawnPositionUtility.TryGetPosition(GetCenter(), minSpawnDistance, maxSpawnDistance, out Vector3 spawnPosition, maxPositionAttempts))
        {
            spawnPosition = GetCenter();
            Debug.LogWarning("BossSpawner: Unable to find NavMesh position, spawning at center.");
        }

        Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        bossSpawned = true;
    }

    // returns the configured spawn center or world origin
    Vector3 GetCenter()
    {
        if (spawnCenter != null)
        {
            return spawnCenter.position;
        }

        return Vector3.zero;
    }

    // clears internal timers to allow another boss spawn
    public void ResetSpawner()
    {
        bossSpawned = false;
        elapsedTime = 0f;
    }
}
