using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float fallbackSpawnInterval = 2f;
    [SerializeField] private int fallbackMaxEnemiesOnScreen = 20;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float lastSpawnTime = 0f;
    private DifficultySettings currentDifficultySettings;
    private float spawnInterval;
    private int maxEnemiesOnScreen;

    void Start()
    {
        spawnInterval = fallbackSpawnInterval;
        maxEnemiesOnScreen = fallbackMaxEnemiesOnScreen;
        UpdateDifficultySettings(true);
        lastSpawnTime = Time.time;
    }

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

    void UpdateDifficultySettings(bool force = false)
    {
        if (GameManager.Instance == null)
        {
            currentDifficultySettings = null;
            spawnInterval = fallbackSpawnInterval;
            maxEnemiesOnScreen = fallbackMaxEnemiesOnScreen;
            return;
        }

        DifficultySettings newSettings = GameManager.Instance.GetCurrentDifficultySettings();

        if (!force && newSettings == currentDifficultySettings)
        {
            return;
        }

        currentDifficultySettings = newSettings;

        if (currentDifficultySettings == null)
        {
            spawnInterval = fallbackSpawnInterval;
            maxEnemiesOnScreen = fallbackMaxEnemiesOnScreen;
            return;
        }

        spawnInterval = currentDifficultySettings.enemySpawnRate > 0f
            ? 1f / currentDifficultySettings.enemySpawnRate
            : float.MaxValue;
        maxEnemiesOnScreen = currentDifficultySettings.maxEnemiesOnScreen;
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        // pick random enemy prefab and spawn point
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // instantiate creates a copy of the prefab at the spawn point
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(enemy);

        // apply difficulty settings to enemy
        if (currentDifficultySettings != null)
        {
            ApplyDifficultyToEnemy(enemy);
        }
    }

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

    void OnDrawGizmosSelected()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 1f);
                }
            }
        }
    }
}

