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

    [Header("Game Settings")]
    [SerializeField] float gameDuration = 300f;
    [SerializeField] Difficulty currentDifficulty = Difficulty.Easy;

    [Header("Scoring")]
    [SerializeField] int pointsPerKill = 10;
    [SerializeField] int pointsPerSecond = 1;
    [SerializeField] int pointsPerPickup = 5;

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

    private float[] difficultyMultipliers = { 1f, 2f, 3f };

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

    void Update()
    {
        if (CurrentState == GameState.Playing && gameStarted)
        {
            UpdateTimer();
            HandlePauseInput();
        }
    }

    void UpdateTimer()
    {
        // deltaTime is time since last frame, subtract it from remaining time
        timeRemaining -= Time.deltaTime;
        // notify listeners that time updated (only if someone is listening)
        OnTimeUpdated?.Invoke(timeRemaining);

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            TriggerVictory();
        }
    }

    void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (CurrentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        gameStarted = true;
        timeRemaining = gameDuration;
        currentScore = 0;
        enemiesKilled = 0;
        itemsCollected = 0;
        Time.timeScale = 1f;
        
        OnScoreUpdated?.Invoke(currentScore);
        OnKillsUpdated?.Invoke(enemiesKilled);
        OnItemsCollectedUpdated?.Invoke(itemsCollected);
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            // timeScale 0 stops all time-based things (animations, movement, etc)
            Time.timeScale = 0f;
            OnGamePaused?.Invoke();
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
            // timeScale 1 means normal speed
            Time.timeScale = 1f;
            OnGameResumed?.Invoke();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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

    public void AddKill()
    {
        enemiesKilled++;
        int points = pointsPerKill;
        AddScore(points);
        OnKillsUpdated?.Invoke(enemiesKilled);
    }

    public void AddItemCollected()
    {
        itemsCollected++;
        int points = pointsPerPickup;
        AddScore(points);
        OnItemsCollectedUpdated?.Invoke(itemsCollected);
    }

    public void AddScore(int points)
    {
        // multiply points by difficulty level (easy=1x, medium=2x, hard=3x)
        float multiplier = difficultyMultipliers[(int)currentDifficulty];
        int finalPoints = Mathf.RoundToInt(points * multiplier);
        currentScore += finalPoints;
        OnScoreUpdated?.Invoke(currentScore);
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        SaveSettings();
    }

    public Difficulty GetDifficulty()
    {
        return currentDifficulty;
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetEnemiesKilled()
    {
        return enemiesKilled;
    }

    public int GetItemsCollected()
    {
        return itemsCollected;
    }

    public float GetDifficultyMultiplier()
    {
        return difficultyMultipliers[(int)currentDifficulty];
    }

    void SaveSettings()
    {
        // playerprefs saves data that persists between game sessions
        PlayerPrefs.SetInt("Difficulty", (int)currentDifficulty);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        // load saved difficulty if it exists
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            currentDifficulty = (Difficulty)PlayerPrefs.GetInt("Difficulty");
        }
    }

    void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}

