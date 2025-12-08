using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// shows the pause menu and routes button presses to the right actions
public class PauseMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button saveGameButton;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    // ensures ui prerequisites exist and wires up the buttons
    void Start()
    {
        // ensure eventsystem exists
        EnsureEventSystem();

        // hide pause menu initially
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // set up button listeners
        SetupButton(resumeButton, ResumeGame, "Resume");
        SetupButton(restartButton, RestartGame, "Restart");
        SetupButton(mainMenuButton, GoToMainMenu, "Main Menu");
        SetupButton(quitButton, QuitGame, "Quit");
        SetupButton(saveGameButton, SaveGame, "Save Game");
    }

    // creates an eventsystem if none exists so buttons can work
    void EnsureEventSystem()
    {
        // check if eventsystem exists in the scene (needed for button clicks)
        if (EventSystem.current == null)
        {
            Debug.LogError("PauseMenuController: No EventSystem found! Creating one...");
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }

    // ensures a button is interactable and hooks up actions plus sounds
    void SetupButton(Button button, UnityEngine.Events.UnityAction action, string buttonName)
    {
        if (button == null)
        {
            Debug.LogWarning($"PauseMenuController: {buttonName} button is not assigned!");
            return;
        }

        // ensure button is interactable
        if (!button.interactable)
        {
            Debug.LogWarning($"PauseMenuController: {buttonName} button is not interactable! Enabling it...");
            button.interactable = true;
        }

        // remove existing listeners to avoid duplicates
        button.onClick.RemoveAllListeners();

        // add listener
        button.onClick.AddListener(action);
        button.onClick.AddListener(PlayButtonSound);
        Debug.Log($"PauseMenuController: {buttonName} button listener added successfully. Button is interactable: {button.interactable}");
    }

    // plays ui audio feedback when a pause button is clicked
    void PlayButtonSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }

    // listens for escape presses to toggle pause state
    void Update()
    {
        if (GameManager.Instance == null || Input.GetKeyDown(KeyCode.Escape) == false)
        {
            return;
        }

        bool pauseMenuVisible = pauseMenuPanel != null && pauseMenuPanel.activeSelf;

        if (pauseMenuVisible && GameManager.Instance.CurrentState == GameState.Paused)
        {
            ResumeGame();
        }
        else if (!pauseMenuVisible && GameManager.Instance.CurrentState == GameState.Playing)
        {
            PauseGame();
        }
    }

    // pauses the game and reveals the menu
    public void PauseGame()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
        {
            GameManager.Instance.PauseGame();
            isPaused = true;
            ShowPauseMenu();
        }
    }

    // resumes gameplay and hides the menu
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

    // reloads the active scene from the pause menu
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

    // navigates back to the main menu scene
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

    // quits the application or editor play mode
    public void QuitGame()
    {
        Debug.Log("PauseMenuController: QuitGame called!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // passes the save request to the save system singleton
    public void SaveGame()
    {
        Debug.Log("PauseMenuController: SaveGame called!");
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
        else
        {
            Debug.LogWarning("PauseMenuController: SaveSystem not available.");
        }
    }

    // activates the pause menu gameobject
    void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    }

    // hides the pause menu gameobject
    void HidePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    // subscribes to game manager pause events when enabled
    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePaused += ShowPauseMenu;
            GameManager.Instance.OnGameResumed += HidePauseMenu;
        }
    }

    // unsubscribes from pause events when disabled
    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePaused -= ShowPauseMenu;
            GameManager.Instance.OnGameResumed -= HidePauseMenu;
        }
    }

    // cleans up button listeners when destroyed
    void OnDestroy()
    {
        // clean up button listeners
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

        if (saveGameButton != null)
        {
            saveGameButton.onClick.RemoveAllListeners();
        }
    }
}

