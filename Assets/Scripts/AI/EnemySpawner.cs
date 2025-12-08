using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

// controls enemy instantiation frequency and difficulty driven stats
public class EnemySpawner : MonoBehaviour
{
    private static readonly Dictionary<string, GameObject> prefabLookup = new Dictionary<string, GameObject>(StringComparer.Ordinal);

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float fallbackSpawnInterval = 10f;
    [SerializeField] private int fallbackMaxEnemiesOnScreen = 10;

    [Header("Random Spawn Area")]
    [SerializeField] private bool useRandomSpawnPositions = true;
    [SerializeField] private Transform spawnCenter;
    [SerializeField] private float minSpawnDistance = 35f;
    [SerializeField] private float maxSpawnDistance = 45f;
    [SerializeField] private int maxPositionAttempts = 10;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float lastSpawnTime = 0f;
    private DifficultySettings currentDifficultySettings;
    private float spawnInterval;
    private int maxEnemiesOnScreen;
    private Difficulty? cachedDifficulty = null;

    // keeps local references of supplied prefabs for lookup
    void Awake()
    {
        RegisterOwnPrefabs();
    }

    // seeds timing values and reads the active difficulty
    void Start()
    {
        spawnInterval = fallbackSpawnInterval;
        maxEnemiesOnScreen = fallbackMaxEnemiesOnScreen;
        UpdateDifficultySettings(true);
        lastSpawnTime = Time.time;
    }

    // handles timed spawning and removes destroyed enemies from tracking
    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        UpdateDifficultySettings();

        // remove all null enemies from list (enemies that were destroyed)
        activeEnemies.RemoveAll(enemy => enemy == null);

        // spawn enemies based on difficulty if enough time passed and not at max
        if (Time.time - lastSpawnTime >= spawnInterval && activeEnemies.Count < maxEnemiesOnScreen)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }

    // refreshes spawn pacing and counts based on difficulty
    void UpdateDifficultySettings(bool force = false)
    {
        if (GameManager.Instance == null)
        {
            currentDifficultySettings = null;
            spawnInterval = fallbackSpawnInterval;
            maxEnemiesOnScreen = fallbackMaxEnemiesOnScreen;
            cachedDifficulty = null;
            return;
        }

        Difficulty selectedDifficulty = GetSavedDifficulty();
        bool difficultyChanged = !cachedDifficulty.HasValue || cachedDifficulty.Value != selectedDifficulty;

        DifficultySettings newSettings = GameManager.Instance.GetDifficultySettings(selectedDifficulty);

        if (!force && !difficultyChanged && newSettings == currentDifficultySettings)
        {
            return;
        }

        cachedDifficulty = selectedDifficulty;
        currentDifficultySettings = newSettings;

        if (currentDifficultySettings == null)
        {
            spawnInterval = GetIntervalForDifficulty(selectedDifficulty);
            maxEnemiesOnScreen = fallbackMaxEnemiesOnScreen;
            return;
        }

        spawnInterval = GetIntervalForDifficulty(selectedDifficulty);
        maxEnemiesOnScreen = currentDifficultySettings.maxEnemiesOnScreen;
    }

    // maps difficulty to the desired spawn interval
    float GetIntervalForDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return 10f;
            case Difficulty.Medium:
                return 7f;
            case Difficulty.Hard:
                return 5f;
            default:
                return fallbackSpawnInterval;
        }
    }

    // chooses a spawn location and instantiates a random enemy
    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector3 spawnPosition;
        Quaternion spawnRotation = Quaternion.identity;

        if (!useRandomSpawnPositions && spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (spawnPoint == null)
            {
                return;
            }

            spawnPosition = spawnPoint.position;
            spawnRotation = spawnPoint.rotation;
        }
        else
        {
            if (!TryGetRandomSpawnPosition(out spawnPosition))
            {
                Debug.LogWarning("EnemySpawner: Failed to find a valid spawn position.");
                return;
            }
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        activeEnemies.Add(enemy);

        // apply difficulty settings to enemy
        if (currentDifficultySettings != null)
        {
            ApplyDifficultyToEnemy(enemy);
        }
    }

    // updates spawned enemy stats based on difficulty settings
    void ApplyDifficultyToEnemy(GameObject enemy)
    {
        if (currentDifficultySettings == null) return;

        // apply speed
        UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = currentDifficultySettings.enemySpeed;
        }

        // apply health and damage
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            // you may need to add public setters to enemyai for these
            // for now we will modify the actor component
        }

        Actor actor = enemy.GetComponent<Actor>();
        if (actor != null)
        {
            actor.SetMaxHealth(currentDifficultySettings.enemyHealth);
        }
    }

    // destroys every tracked enemy instance immediately
    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
    }

    // draws spawn ranges and point markers inside the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (spawnCenter != null)
        {
            Gizmos.DrawWireSphere(spawnCenter.position, minSpawnDistance);
            Gizmos.DrawWireSphere(spawnCenter.position, maxSpawnDistance);
        }

        if (spawnPoints != null)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireCube(spawnPoint.position, Vector3.one);
                }
            }
        }
    }

    // locates a navmesh position within the configured ring
    bool TryGetRandomSpawnPosition(out Vector3 position)
    {
        float minDistance = Mathf.Max(0f, minSpawnDistance);
        float maxDistance = Mathf.Max(minDistance + 1f, maxSpawnDistance);
        Vector3 center = spawnCenter != null ? spawnCenter.position : Vector3.zero;

        return SpawnPositionUtility.TryGetPosition(center, minDistance, maxDistance, out position, maxPositionAttempts);
    }

    // reads the saved difficulty from player prefs
    Difficulty GetSavedDifficulty()
    {
        return (Difficulty)PlayerPrefs.GetInt("Difficulty", (int)Difficulty.Easy);
    }

    // ensures this spawner's prefabs are available in the lookup
    void RegisterOwnPrefabs()
    {
        RegisterPrefabs(enemyPrefabs);
    }

    // adds provided prefabs to the shared dictionary
    static void RegisterPrefabs(GameObject[] prefabs)
    {
        if (prefabs == null)
        {
            return;
        }

        foreach (var prefab in prefabs)
        {
            if (prefab == null)
            {
                continue;
            }

            string key = prefab.name;
            if (!prefabLookup.ContainsKey(key))
            {
                prefabLookup.Add(key, prefab);
            }
        }
    }

    // asks all spawners to repopulate the lookup dictionary
    public static void RefreshPrefabLookup()
    {
        EnemySpawner[] spawners = GameObject.FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.RegisterOwnPrefabs();
            }
        }
    }

    // returns a prefab reference if it was previously registered
    public static GameObject GetRegisteredPrefab(string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            return null;
        }

        prefabLookup.TryGetValue(prefabName, out GameObject prefab);
        return prefab;
    }
}

