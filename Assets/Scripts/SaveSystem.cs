using System;
using System.Collections;
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

    // coroutine waits for scene to load before restoring player state
    IEnumerator RestoreGameState(GameData data)
    {
        const float maxSceneWait = 10f;
        float sceneWaitTimer = 0f;
        while (SceneManager.GetActiveScene().name != data.sceneName && sceneWaitTimer < maxSceneWait)
        {
            sceneWaitTimer += Time.unscaledDeltaTime;
            yield return null;
        }

        GameObject player = null;
        float waitTime = 0f;
        const float maxPlayerWait = 5f;
        while (player == null && waitTime < maxPlayerWait)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (player != null)
        {
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

        while (GameManager.Instance == null)
        {
            yield return null;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ApplyLoadedGameData(data);
        }

        isRestoringSave = false;
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
    }
}

