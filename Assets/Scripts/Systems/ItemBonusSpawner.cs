using System.Collections.Generic;
using UnityEngine;

// periodically spawns random item pickups at predefined points
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

    // gathers child spawn points if none were assigned
    void Awake()
    {
        CacheChildSpawnPointsIfNeeded();
    }

    // schedules the first spawn when enabled
    void OnEnable()
    {
        ScheduleNextSpawn(spawnImmediately ? 0f : spawnIntervalSeconds);
    }

    // checks whether it is time to spawn another bonus
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            TrySpawnBonus();
        }
    }

    // enforces limits and instantiates a random bonus prefab
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

    // removes destroyed bonuses from the tracking list
    void CleanupActiveBonuses()
    {
        activeBonuses.RemoveAll(bonus => bonus == null);
    }

    // verifies that at least one prefab is available
    bool HasValidPrefabs()
    {
        return bonusPrefabs != null && bonusPrefabs.Length > 0;
    }

    // ensures there are spawn points, attempting to cache children if needed
    bool HasSpawnPoints()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            return true;
        }

        CacheChildSpawnPointsIfNeeded();
        return spawnPoints != null && spawnPoints.Length > 0;
    }

    // sets the timestamp for the next spawn attempt
    void ScheduleNextSpawn(float delay)
    {
        nextSpawnTime = Time.time + Mathf.Max(0f, delay);
    }

    // uses child transforms as spawn points if none were manually provided
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

    // draws helper gizmos showing where bonuses can appear
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
