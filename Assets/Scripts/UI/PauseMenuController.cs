using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private GameObject optionsMenuPanel; // Reference to options menu if it exists

    private bool isPaused = false;

    void Start()
    {
        // Ensure EventSystem exists
        EnsureEventSystem();
        
        // Hide pause menu initially
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Setup button listeners
        SetupButton(resumeButton, ResumeGame, "Resume");
        SetupButton(restartButton, RestartGame, "Restart");
        SetupButton(mainMenuButton, GoToMainMenu, "Main Menu");
        SetupButton(quitButton, QuitGame, "Quit");
    }

    void EnsureEventSystem()
    {
        // Check if EventSystem exists in the scene
        if (EventSystem.current == null)
        {
            Debug.LogError("PauseMenuController: No EventSystem found! Creating one...");
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }

    void SetupButton(Button button, UnityEngine.Events.UnityAction action, string buttonName)
    {
        if (button == null)
        {
            Debug.LogWarning($"PauseMenuController: {buttonName} button is not assigned!");
            return;
        }

        // Ensure button is interactable
        if (!button.interactable)
        {
            Debug.LogWarning($"PauseMenuController: {buttonName} button is not interactable! Enabling it...");
            button.interactable = true;
        }

        // Remove existing listeners to avoid duplicates
        button.onClick.RemoveAllListeners();
        
        // Add listener
        button.onClick.AddListener(action);
        Debug.Log($"PauseMenuController: {buttonName} button listener added successfully. Button is interactable: {button.interactable}");
    }

    void Update()
    {
        // Don't handle Esc if options menu is open (let OptionsMenuController handle it)
        if (optionsMenuPanel != null && optionsMenuPanel.activeSelf)
        {
            return;
        }

        // Check for pause input (handled by GameManager, but we can also check here as backup)
        // Only handle Esc if pause menu is visible or game is playing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && pauseMenuPanel != null && pauseMenuPanel.activeSelf)
            {
                ResumeGame();
            }
            else if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
        {
            GameManager.Instance.PauseGame();
            isPaused = true;
            ShowPauseMenu();
        }
    }

    public void ResumeGame()
    {
        Debug.Log("PauseMenuController: ResumeGame called!");
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Paused)
        {
            GameManager.Instance.ResumeGame();
            isPaused = false;
            HidePauseMenu();
        }
    }

    public void RestartGame()
    {
        Debug.Log("PauseMenuController: RestartGame called!");
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
        Debug.Log("PauseMenuController: GoToMainMenu called!");
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
        Debug.Log("PauseMenuController: QuitGame called!");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    }

    void HidePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePaused += ShowPauseMenu;
            GameManager.Instance.OnGameResumed += HidePauseMenu;
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePaused -= ShowPauseMenu;
            GameManager.Instance.OnGameResumed -= HidePauseMenu;
        }
    }

    void OnDestroy()
    {
        // Clean up button listeners
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
        }

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
}

