using UnityEngine;

/// <summary>
/// Spawns the boss a set time after gameplay starts, ensuring it appears far from the arena center.
/// </summary>
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

    void OnEnable()
    {
        ResetSpawner();
    }

    void OnDisable()
    {
        ResetSpawner();
    }

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

    Vector3 GetCenter()
    {
        if (spawnCenter != null)
        {
            return spawnCenter.position;
        }

        return Vector3.zero;
    }

    public void ResetSpawner()
    {
        bossSpawned = false;
        elapsedTime = 0f;
    }
}
