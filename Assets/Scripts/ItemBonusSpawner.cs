using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Periodically spawns random item pickups at predefined spawn points.
/// Intended to be paired with the Bonus Item Spawner tool.
/// </summary>
public class ItemBonusSpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]
    [SerializeField] private ItemPickup[] bonusPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField, Min(1f)] private float spawnIntervalSeconds = 15f;
    [SerializeField, Range(1, 6)] private int maxSimultaneousBonuses = 1;
    [SerializeField] private bool spawnImmediately = true;

    private readonly List<GameObject> activeBonuses = new List<GameObject>();
    private float nextSpawnTime;

    void Awake()
    {
        CacheChildSpawnPointsIfNeeded();
    }

    void OnEnable()
    {
        ScheduleNextSpawn(spawnImmediately ? 0f : spawnIntervalSeconds);
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            TrySpawnBonus();
        }
    }

    void TrySpawnBonus()
    {
        CleanupActiveBonuses();

        if (!HasValidPrefabs() || !HasSpawnPoints())
        {
            ScheduleNextSpawn(spawnIntervalSeconds);
            return;
        }

        if (activeBonuses.Count >= maxSimultaneousBonuses)
        {
            ScheduleNextSpawn(spawnIntervalSeconds);
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        ItemPickup prefab = bonusPrefabs[Random.Range(0, bonusPrefabs.Length)];

        if (spawnPoint == null || prefab == null)
        {
            ScheduleNextSpawn(spawnIntervalSeconds);
            return;
        }

        GameObject spawned = Instantiate(prefab.gameObject, spawnPoint.position, spawnPoint.rotation);
        activeBonuses.Add(spawned);

        ScheduleNextSpawn(spawnIntervalSeconds);
    }

    void CleanupActiveBonuses()
    {
        activeBonuses.RemoveAll(bonus => bonus == null);
    }

    bool HasValidPrefabs()
    {
        return bonusPrefabs != null && bonusPrefabs.Length > 0;
    }

    bool HasSpawnPoints()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            return true;
        }

        CacheChildSpawnPointsIfNeeded();
        return spawnPoints != null && spawnPoints.Length > 0;
    }

    void ScheduleNextSpawn(float delay)
    {
        nextSpawnTime = Time.time + Mathf.Max(0f, delay);
    }

    void CacheChildSpawnPointsIfNeeded()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            return;
        }

        List<Transform> collected = new List<Transform>();
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child != transform)
            {
                collected.Add(child);
            }
        }

        if (collected.Count > 0)
        {
            spawnPoints = collected.ToArray();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (spawnPoints == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        foreach (Transform point in spawnPoints)
        {
            if (point == null)
            {
                continue;
            }
            Gizmos.DrawWireSphere(point.position, 0.5f);
        }
    }
}
