using System.IO;
using UnityEngine;
using System;

// data structure that holds all game state to save
[Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public int playerHealth;
    public int currentScore;
    public float timeRemaining;
    public int difficulty;
    public string sceneName;
    public int enemiesKilled;
    public int itemsCollected;
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
            Actor playerActor = player.GetComponent<Actor>();
            if (playerActor != null)
            {
                data.playerHealth = playerActor.currentHealth;
            }
        }

        data.currentScore = GameManager.Instance.GetCurrentScore();
        data.timeRemaining = GameManager.Instance.GetTimeRemaining();
        data.difficulty = (int)GameManager.Instance.GetDifficulty();
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
            return false;
        }
    }

    // coroutine waits for scene to load before restoring player state
    System.Collections.IEnumerator RestoreGameState(GameData data)
    {
        yield return new WaitForSeconds(0.5f); // wait for scene to finish loading

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = data.playerPosition;
            Actor playerActor = player.GetComponent<Actor>();
            if (playerActor != null)
            {
                playerActor.SetMaxHealth(data.playerHealth);
                playerActor.Heal(data.playerHealth);
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty((Difficulty)data.difficulty);
        }
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

