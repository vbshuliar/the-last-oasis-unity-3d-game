using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// handles showing the victory or game over menu and related actions
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

    // hides the menu and wires up button callbacks
    void Start()
    {
        hasShownEndMenu = false;
        // hide menu initially
        if (endMenuPanel != null)
        {
            endMenuPanel.SetActive(false);
        }

        // set up button listeners
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

    // subscribes to game manager events when enabled
    void OnEnable()
    {
        hasShownEndMenu = false;
        TrySubscribeToGameManager();
    }

    // unsubscribes when disabled to avoid leaks
    void OnDisable()
    {
        TryUnsubscribeFromGameManager();
    }

    // listens for victory state to automatically show the menu
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

    // configures the menu for defeat
    void ShowGameOverMenu()
    {
        ShowEndMenu(false);
    }

    // configures the menu for victory
    void ShowVictoryMenu()
    {
        ShowEndMenu(true);
    }

    // updates title and stats before displaying the panel
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

        // update title
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

    // restarts the active scene and hides the menu
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

    // loads the configured main menu scene
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

    // exits the application or stops play mode in the editor
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // removes listeners when the object is destroyed
    void OnDestroy()
    {
        TryUnsubscribeFromGameManager();
        // clean up button listeners
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

    // attaches to game manager events exactly once
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

    // detaches from game manager events safely
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

