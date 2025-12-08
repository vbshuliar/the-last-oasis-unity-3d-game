using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EndMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject endMenuPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool hasShownEndMenu = false;
    private bool isSubscribedToGameManager = false;

    void Start()
    {
        hasShownEndMenu = false;
        // Hide menu initially
        if (endMenuPanel != null)
        {
            endMenuPanel.SetActive(false);
        }

        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    void OnEnable()
    {
        hasShownEndMenu = false;
        TrySubscribeToGameManager();
    }

    void OnDisable()
    {
        TryUnsubscribeFromGameManager();
    }

    void Update()
    {
        if (!isSubscribedToGameManager)
        {
            TrySubscribeToGameManager();
        }

        if (!hasShownEndMenu && GameManager.Instance != null &&
            GameManager.Instance.CurrentState == GameState.Victory &&
            GameManager.Instance.GetTimeRemaining() <= 0f)
        {
            ShowEndMenu(true);
        }
    }

    void ShowGameOverMenu()
    {
        ShowEndMenu(false);
    }

    void ShowVictoryMenu()
    {
        ShowEndMenu(true);
    }

    void ShowEndMenu(bool isVictory)
    {
        if (hasShownEndMenu)
        {
            return;
        }

        hasShownEndMenu = true;
        if (endMenuPanel != null)
        {
            endMenuPanel.SetActive(true);
        }

        // Update title
        if (titleText != null)
        {
            titleText.text = isVictory ? "VICTORY!" : "GAME OVER";
        }

        // update stats
        if (GameManager.Instance != null)
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: " + GameManager.Instance.GetCurrentScore().ToString();
            }

            if (timeText != null)
            {
                float timeSurvived = 180f - GameManager.Instance.GetTimeRemaining();
                int minutes = Mathf.FloorToInt(timeSurvived / 60);
                int seconds = Mathf.FloorToInt(timeSurvived % 60);
                timeText.text = "Time Survived: " + string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            if (killsText != null)
            {
                killsText.text = "Enemies Killed: " + GameManager.Instance.GetEnemiesKilled().ToString();
            }

            if (highScoreText != null)
            {
                int highScore = GameManager.Instance.GetHighScore();
                highScoreText.text = "High Score: " + highScore.ToString();

                // show if new high score
                if (GameManager.Instance.GetCurrentScore() >= highScore && highScore > 0)
                {
                    highScoreText.text += " (NEW!)";
                }
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void OnDestroy()
    {
        TryUnsubscribeFromGameManager();
        // Clean up button listeners
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
        }
    }

    void TrySubscribeToGameManager()
    {
        if (isSubscribedToGameManager || GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.OnGameOver += ShowGameOverMenu;
        GameManager.Instance.OnVictory += ShowVictoryMenu;
        isSubscribedToGameManager = true;
    }

    void TryUnsubscribeFromGameManager()
    {
        if (!isSubscribedToGameManager || GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.OnGameOver -= ShowGameOverMenu;
        GameManager.Instance.OnVictory -= ShowVictoryMenu;
        isSubscribedToGameManager = false;
    }
}

