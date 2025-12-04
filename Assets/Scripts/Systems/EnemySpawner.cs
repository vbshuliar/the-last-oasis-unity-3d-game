using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxEnemiesOnScreen = 20;

    [Header("Difficulty Settings")]
    [SerializeField] private DifficultySettings easySettings;
    [SerializeField] private DifficultySettings mediumSettings;
    [SerializeField] private DifficultySettings hardSettings;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float lastSpawnTime = 0f;
    private DifficultySettings currentDifficultySettings;

    void Start()
    {
        UpdateDifficultySettings();
        lastSpawnTime = Time.time;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        UpdateDifficultySettings();

        // removeall with lambda removes all null enemies from list
        activeEnemies.RemoveAll(enemy => enemy == null);

        // spawn enemies based on difficulty if enough time passed and not at max
        if (Time.time - lastSpawnTime >= spawnInterval && activeEnemies.Count < maxEnemiesOnScreen)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }

    void UpdateDifficultySettings()
    {
        if (GameManager.Instance == null) return;

        Difficulty currentDifficulty = GameManager.Instance.GetDifficulty();

        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                currentDifficultySettings = easySettings;
                break;
            case Difficulty.Medium:
                currentDifficultySettings = mediumSettings;
                break;
            case Difficulty.Hard:
                currentDifficultySettings = hardSettings;
                break;
        }

        if (currentDifficultySettings != null)
        {
            spawnInterval = 1f / currentDifficultySettings.enemySpawnRate;
            maxEnemiesOnScreen = currentDifficultySettings.maxEnemiesOnScreen;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        // random range picks random number between min inclusive and max exclusive
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

