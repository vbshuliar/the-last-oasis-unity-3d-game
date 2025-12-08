using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

// data structure that holds all game state to save
[Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public int playerHealth;
    public int playerMaxHealth;
    public int currentScore;
    public float timeRemaining;
    public int enemiesKilled;
    public int itemsCollected;
    public string sceneName;
    public EnemyStateData[] enemies;
}

[Serializable]
public class EnemyStateData
{
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public int currentHealth;
    public int maxHealth;
}

// handles saving and loading game state to from a file
public class SaveSystem : MonoBehaviour
{
    private static SaveSystem instance;
    public static SaveSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SaveSystem");
                instance = go.AddComponent<SaveSystem>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private string saveFilePath;
    private bool isRestoringSave;

    public bool IsRestoringSave => isRestoringSave;

    // establishes the singleton instance and determines save file path
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // writes the current game state to disk as json
    public void SaveGame()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager not found. Cannot save game.");
            return;
        }

        GameData data = new GameData();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosition = player.transform.position;
            data.playerRotation = player.transform.rotation;
            Actor playerActor = player.GetComponent<Actor>();
            if (playerActor != null)
            {
                data.playerHealth = playerActor.currentHealth;
                data.playerMaxHealth = playerActor.maxHealth;
            }
        }

        data.currentScore = GameManager.Instance.GetCurrentScore();
        data.timeRemaining = GameManager.Instance.GetTimeRemaining();
        data.enemiesKilled = GameManager.Instance.GetEnemiesKilled();
        data.itemsCollected = GameManager.Instance.GetItemsCollected();
        data.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        data.enemies = CaptureEnemyStates();

        // convert game data to json format text that can be saved to file
        string json = JsonUtility.ToJson(data, true);
        try
        {
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Game saved successfully to: " + saveFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    // loads game state from disk and begins restoring it
    public bool LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save file found.");
            return false;
        }

        try
        {
            // read json file and convert back to game data object
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            isRestoringSave = true;

            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadScene(data.sceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(data.sceneName);
            }

            StartCoroutine(RestoreGameState(data));

            Debug.Log("Game loaded successfully from: " + saveFilePath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load game: " + e.Message);
            isRestoringSave = false;
            return false;
        }
    }

    // waits for the scene to load before restoring actors
    IEnumerator RestoreGameState(GameData data)
    {
        yield return WaitForSceneToLoad(data.sceneName);
        yield return RestorePlayerState(data);
        yield return RestoreEnemyStates(data.enemies);

        while (GameManager.Instance == null)
        {
            yield return null;
        }

        GameManager.Instance.ApplyLoadedGameData(data);
        isRestoringSave = false;
    }

    // repeatedly checks if the requested scene has finished loading
    IEnumerator WaitForSceneToLoad(string sceneName)
    {
        const float maxSceneWait = 10f;
        float sceneWaitTimer = 0f;
        while (SceneManager.GetActiveScene().name != sceneName && sceneWaitTimer < maxSceneWait)
        {
            sceneWaitTimer += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    // restores the player position rotation and health
    IEnumerator RestorePlayerState(GameData data)
    {
        GameObject player = null;
        float waitTime = 0f;
        const float maxPlayerWait = 5f;
        while (player == null && waitTime < maxPlayerWait)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (player == null)
        {
            yield break;
        }

        NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(data.playerPosition);
        }
        else
        {
            player.transform.position = data.playerPosition;
        }

        Quaternion targetRotation = data.playerRotation;
        bool legacyRotation = Mathf.Approximately(targetRotation.x, 0f)
            && Mathf.Approximately(targetRotation.y, 0f)
            && Mathf.Approximately(targetRotation.z, 0f)
            && Mathf.Approximately(targetRotation.w, 0f);
        if (legacyRotation)
        {
            targetRotation = player.transform.rotation;
        }
        player.transform.rotation = targetRotation;

        Actor playerActor = player.GetComponent<Actor>();
        if (playerActor != null)
        {
            int restoredMaxHealth = data.playerMaxHealth > 0 ? data.playerMaxHealth : playerActor.maxHealth;
            playerActor.SetMaxHealth(restoredMaxHealth);

            bool legacySave = data.playerMaxHealth == 0 && data.playerHealth == 0;
            int restoredCurrentHealth = legacySave ? restoredMaxHealth : data.playerHealth;
            playerActor.SetCurrentHealth(restoredCurrentHealth);
        }
    }

    // recreates or reuses enemies using serialized state data
    IEnumerator RestoreEnemyStates(EnemyStateData[] enemyStates)
    {
        if (enemyStates == null || enemyStates.Length == 0)
        {
            yield break;
        }

        EnemySpawner.RefreshPrefabLookup();
        List<GameObject> reusableEnemies = CollectExistingEnemies();

        foreach (var enemyState in enemyStates)
        {
            if (enemyState == null)
            {
                continue;
            }

            GameObject enemyInstance = InstantiateEnemyFromState(enemyState);
            if (enemyInstance == null)
            {
                enemyInstance = ReuseExistingEnemy(reusableEnemies, enemyState.prefabName);
                if (enemyInstance == null)
                {
                    Debug.LogWarning($"SaveSystem: Unable to restore enemy '{enemyState.prefabName}'. No prefab or reusable instance available.");
                    continue;
                }
            }

            ApplyEnemyTransform(enemyInstance, enemyState);
            ApplyEnemyHealth(enemyInstance, enemyState);
        }

        foreach (var leftover in reusableEnemies)
        {
            if (leftover != null)
            {
                Destroy(leftover);
            }
        }
    }

    // gathers any currently spawned enemies so they can be reused
    List<GameObject> CollectExistingEnemies()
    {
        EnemyAI[] existing = GameObject.FindObjectsOfType<EnemyAI>();
        var list = new List<GameObject>(existing.Length);
        foreach (var enemy in existing)
        {
            if (enemy != null)
            {
                list.Add(enemy.gameObject);
            }
        }
        return list;
    }

    // tries to instantiate an enemy prefab that matches the saved data
    GameObject InstantiateEnemyFromState(EnemyStateData state)
    {
        if (state == null)
        {
            return null;
        }

        GameObject prefab = EnemySpawner.GetRegisteredPrefab(state.prefabName);
        if (prefab == null)
        {
            return null;
        }

        return Instantiate(prefab, state.position, state.rotation);
    }

    // falls back to reusing an already spawned enemy when prefab lookup fails
    GameObject ReuseExistingEnemy(List<GameObject> pool, string prefabName)
    {
        if (pool == null || pool.Count == 0)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(prefabName))
        {
            for (int i = 0; i < pool.Count; i++)
            {
                GameObject candidate = pool[i];
                if (candidate == null)
                {
                    continue;
                }

                if (string.Equals(GetPrefabIdentifier(candidate), prefabName, StringComparison.Ordinal))
                {
                    pool.RemoveAt(i);
                    return candidate;
                }
            }
        }

        GameObject fallback = pool[0];
        pool.RemoveAt(0);
        return fallback;
    }

    // sets enemy health values according to the saved state
    void ApplyEnemyHealth(GameObject enemyInstance, EnemyStateData state)
    {
        if (enemyInstance == null || state == null)
        {
            return;
        }

        Actor enemyActor = enemyInstance.GetComponent<Actor>();
        if (enemyActor == null)
        {
            return;
        }

        if (state.maxHealth > 0)
        {
            enemyActor.SetMaxHealth(state.maxHealth);
        }

        int healthToSet = (state.maxHealth == 0 && state.currentHealth == 0)
            ? enemyActor.maxHealth
            : state.currentHealth;
        enemyActor.SetCurrentHealth(Mathf.Clamp(healthToSet, 0, enemyActor.maxHealth));
    }

    // positions the enemy on the navmesh and restores its rotation
    void ApplyEnemyTransform(GameObject enemyInstance, EnemyStateData state)
    {
        if (enemyInstance == null || state == null)
        {
            return;
        }

        NavMeshAgent agent = enemyInstance.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(state.position);
        }
        else
        {
            enemyInstance.transform.position = state.position;
        }

        enemyInstance.transform.rotation = state.rotation;
    }

    // reports whether a save file currently exists
    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    // deletes the existing save file, if one is present
    public void DeleteSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
    }

    // serializes all active enemies into lightweight state data
    EnemyStateData[] CaptureEnemyStates()
    {
        EnemyAI[] enemies = GameObject.FindObjectsOfType<EnemyAI>();
        if (enemies == null || enemies.Length == 0)
        {
            return Array.Empty<EnemyStateData>();
        }

        var stateList = new List<EnemyStateData>(enemies.Length);
        foreach (var enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            GameObject enemyObject = enemy.gameObject;
            Actor actor = enemyObject.GetComponent<Actor>();
            var state = new EnemyStateData
            {
                prefabName = GetPrefabIdentifier(enemyObject),
                position = enemyObject.transform.position,
                rotation = enemyObject.transform.rotation,
                currentHealth = actor != null ? actor.currentHealth : 0,
                maxHealth = actor != null ? actor.maxHealth : 0
            };
            stateList.Add(state);
        }

        return stateList.ToArray();
    }

    // derives the prefab name from a spawned enemy instance
    string GetPrefabIdentifier(GameObject source)
    {
        if (source == null)
        {
            return string.Empty;
        }

        string name = source.name;
        const string cloneSuffix = "(Clone)";
        int cloneIndex = name.IndexOf(cloneSuffix, StringComparison.Ordinal);
        if (cloneIndex >= 0)
        {
            name = name.Substring(0, cloneIndex);
        }

        return name.Trim();
    }
}

