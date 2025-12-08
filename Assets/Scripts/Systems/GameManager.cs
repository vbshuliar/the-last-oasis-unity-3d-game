using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Victory
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

// main game manager that handles game state timer scoring and win lose conditions
public class GameManager : MonoBehaviour
{
    // singleton pattern only one gamemanager exists accessible from anywhere
    public static GameManager Instance { get; private set; }

    private const float GameDurationSeconds = 180f;

    [Header("Game Settings")]
    [SerializeField] Difficulty currentDifficulty = Difficulty.Easy;

    [Header("Scoring")]
    [SerializeField] int pointsPerKill = 10;
    [SerializeField] int pointsPerSecond = 1;
    [SerializeField] int pointsPerPickup = 5;

    [Header("Difficulty Profiles")]
    [SerializeField] DifficultySettings easyDifficultySettings;
    [SerializeField] DifficultySettings mediumDifficultySettings;
    [SerializeField] DifficultySettings hardDifficultySettings;

    public GameState CurrentState { get; private set; }
    private float timeRemaining;
    private int currentScore = 0;
    private int enemiesKilled = 0;
    private int itemsCollected = 0;
    private bool gameStarted = false;

    // events let other scripts know when things happen ui can listen and update
    public event Action<float> OnTimeUpdated;
    public event Action<int> OnScoreUpdated;
    public event Action<int> OnKillsUpdated;
    public event Action<int> OnItemsCollectedUpdated;
    public event Action OnGameOver;
    public event Action OnVictory;
    public event Action OnGamePaused;
    public event Action OnGameResumed;

    // initializes the singleton instance and loads saved settings
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep this object alive when loading new scenes
        }
        else
        {
            Destroy(gameObject); // destroy duplicate if one already exists
            return;
        }

        CurrentState = GameState.MainMenu;
        LoadSettings();
    }

    // counts down the timer whenever the game is running
    void Update()
    {
        if (CurrentState == GameState.Playing && gameStarted)
        {
            UpdateTimer();
        }
    }

    // decrements the remaining time and fires victory when it runs out
    void UpdateTimer()
    {
        // delta time is time since last frame, subtract it from remaining time
        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(0f, timeRemaining); // never expose negative time

        // notify listeners that time updated (only if someone is listening)
        OnTimeUpdated?.Invoke(timeRemaining);

        if (timeRemaining <= 0)
        {
            TriggerVictory();
        }
    }

    // listens for escape key presses to toggle pause state
    void HandlePauseInput()
    {
        // only handle pause input when playing - let pausemenucontroller handle resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == GameState.Playing)
            {
                PauseGame();
            }
            // don't handle esc when paused - pausemenucontroller will handle it
        }
    }

    // resets score counters and begins the play session
    public void StartGame()
    {
        bool restoringFromSave = SaveSystem.Instance != null && SaveSystem.Instance.IsRestoringSave;
        if (restoringFromSave)
        {
            // skip reset when a save load is about to reapply state.
            return;
        }
        CurrentState = GameState.Playing;
        gameStarted = true;
        timeRemaining = GameDurationSeconds;
        currentScore = 0;
        enemiesKilled = 0;
        itemsCollected = 0;
        Time.timeScale = 1f;

        OnScoreUpdated?.Invoke(currentScore);
        OnKillsUpdated?.Invoke(enemiesKilled);
        OnItemsCollectedUpdated?.Invoke(itemsCollected);
    }

    // restores state values copied from disk
    public void ApplyLoadedGameData(GameData data)
    {
        if (data == null)
        {
            return;
        }

        CurrentState = GameState.Playing;
        gameStarted = true;

        timeRemaining = Mathf.Clamp(data.timeRemaining, 0f, GameDurationSeconds);
        currentScore = Mathf.Max(0, data.currentScore);
        enemiesKilled = Mathf.Max(0, data.enemiesKilled);
        itemsCollected = Mathf.Max(0, data.itemsCollected);

        OnTimeUpdated?.Invoke(timeRemaining);
        OnScoreUpdated?.Invoke(currentScore);
        OnKillsUpdated?.Invoke(enemiesKilled);
        OnItemsCollectedUpdated?.Invoke(itemsCollected);

        Time.timeScale = 1f;
    }

    // switches the state to paused and freezes time
    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            // time scale zero stops all time based things (animations, movement, etc)
            Time.timeScale = 0f;
            OnGamePaused?.Invoke();
        }
    }

    // resumes normal gameplay after a pause
    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
            // time scale one means normal speed
            Time.timeScale = 1f;
            OnGameResumed?.Invoke();
        }
    }

    // reloads the active scene for a fresh attempt
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // transitions into the game over state and saves high score data
    public void GameOver()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.GameOver;
            Time.timeScale = 0f;
            OnGameOver?.Invoke();
            SaveHighScore();
        }
    }

    // transitions into the victory state when time expires
    public void TriggerVictory()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Victory;
            Time.timeScale = 0f;
            OnVictory?.Invoke();
            SaveHighScore();
        }
    }

    // increments kill count and awards score
    public void AddKill()
    {
        enemiesKilled++;
        int points = pointsPerKill;
        AddScore(points);
        OnKillsUpdated?.Invoke(enemiesKilled);
    }

    // increments item count and awards score
    public void AddItemCollected()
    {
        itemsCollected++;
        int points = pointsPerPickup;
        AddScore(points);
        OnItemsCollectedUpdated?.Invoke(itemsCollected);
    }

    // applies difficulty multipliers before increasing score
    public void AddScore(int points)
    {
        // multiply points by difficulty level (easy 1x, medium 2x, hard 3x)
        float multiplier = GetCurrentScoreMultiplier();
        int finalPoints = Mathf.RoundToInt(points * multiplier);
        currentScore += finalPoints;
        OnScoreUpdated?.Invoke(currentScore);
    }

    // stores the newly selected difficulty and persists it
    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        SaveSettings();
    }

    // exposes the active difficulty level
    public Difficulty GetDifficulty()
    {
        return currentDifficulty;
    }

    // returns the remaining round time in seconds
    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    // returns the player's current score
    public int GetCurrentScore()
    {
        return currentScore;
    }

    // returns the number of enemies eliminated
    public int GetEnemiesKilled()
    {
        return enemiesKilled;
    }

    // returns the number of items collected
    public int GetItemsCollected()
    {
        return itemsCollected;
    }

    // helper for ui elements needing the score multiplier
    public float GetDifficultyMultiplier()
    {
        return GetCurrentScoreMultiplier();
    }

    // exposes the fixed game duration in seconds
    public float GetGameDuration()
    {
        return GameDurationSeconds;
    }

    // returns the multiplier from the current difficulty profile
    public float GetCurrentScoreMultiplier()
    {
        DifficultySettings settings = GetCurrentDifficultySettings();
        if (settings == null || settings.scoreMultiplier <= 0f)
        {
            return 1f;
        }

        return settings.scoreMultiplier;
    }

    // grabs the active difficulty settings asset
    public DifficultySettings GetCurrentDifficultySettings()
    {
        return GetDifficultySettings(currentDifficulty);
    }

    // maps difficulties to their respective settings assets
    public DifficultySettings GetDifficultySettings(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return easyDifficultySettings;
            case Difficulty.Medium:
                return mediumDifficultySettings;
            case Difficulty.Hard:
                return hardDifficultySettings;
            default:
                return easyDifficultySettings;
        }
    }

    // uses difficulty to determine the player's max health
    public int GetPlayerMaxHealth(int baseMaxHealth)
    {
        DifficultySettings settings = GetCurrentDifficultySettings();
        float multiplier = (settings != null && settings.playerHealthMultiplier > 0f)
            ? settings.playerHealthMultiplier
            : 1f;

        int adjusted = Mathf.RoundToInt(baseMaxHealth * multiplier);
        return Mathf.Max(1, adjusted);
    }

    // applies health adjustments to the player actor
    public void ApplyDifficultyToPlayer(Actor playerActor, int baseMaxHealth)
    {
        if (playerActor == null)
        {
            return;
        }

        int adjustedHealth = GetPlayerMaxHealth(baseMaxHealth);
        playerActor.SetMaxHealth(adjustedHealth);
        playerActor.Heal(adjustedHealth);
    }

    // saves current difficulty to player prefs
    void SaveSettings()
    {
        // player prefs saves data that persists between game sessions
        PlayerPrefs.SetInt("Difficulty", (int)currentDifficulty);
        PlayerPrefs.Save();
    }

    // restores previously saved difficulty, if any
    void LoadSettings()
    {
        // load saved difficulty if it exists
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            currentDifficulty = (Difficulty)PlayerPrefs.GetInt("Difficulty");
        }
    }

    // writes a new high score to player prefs when appropriate
    void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
    }

    // returns the saved high score value
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    // ensures time scale is restored if the manager is destroyed
    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}

