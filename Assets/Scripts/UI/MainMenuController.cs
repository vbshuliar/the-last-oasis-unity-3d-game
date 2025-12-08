using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// handles main menu button clicks and scene loading
public class MainMenuController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button startLevel1Button;
    [SerializeField] private Button startLevel2Button;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button loadGameButton;

    [Header("Options Menu")]
    [SerializeField] private OptionsMenuController optionsMenuController;
    [SerializeField] private GameObject optionsMenuPanel;

    [Header("Scene Names")]
    [SerializeField] private string level1SceneName = "Level1";
    [SerializeField] private string level2SceneName = "Level2";
    [SerializeField] private string tutorialSceneName = "Tutorial";

    void Start()
    {
        // Ensure EventSystem exists
        EnsureEventSystem();

        // Verify and setup buttons
        SetupButton(startLevel1Button, OnStartLevel1Clicked, "Start Level 1");
        SetupButton(startLevel2Button, OnStartLevel2Clicked, "Start Level 2");
        SetupButton(tutorialButton, OnTutorialClicked, "Tutorial");
        SetupButton(optionsButton, OnOptionsClicked, "Options");
        SetupButton(quitButton, OnQuitClicked, "Quit");
        SetupButton(loadGameButton, OnLoadGameClicked, "Load Game");

        Time.timeScale = 1f;
    }

    void EnsureEventSystem()
    {
        // check if eventsystem exists in the scene (needed for button clicks)
        if (EventSystem.current == null)
        {
            Debug.LogError("MainMenuController: No EventSystem found! Creating one...");
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
        else
        {
            Debug.Log("MainMenuController: EventSystem found");
        }

        // check if canvas has graphicraycaster (needed to detect ui clicks)
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                Debug.LogWarning("MainMenuController: Canvas missing GraphicRaycaster! Adding one...");
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            else
            {
                Debug.Log("MainMenuController: Canvas has GraphicRaycaster");
            }
        }
    }

    void SetupButton(Button button, UnityEngine.Events.UnityAction action, string buttonName)
    {
        if (button == null)
        {
            Debug.LogWarning($"MainMenuController: {buttonName} button is not assigned!");
            return;
        }

        // Ensure button is interactable
        if (!button.interactable)
        {
            Debug.LogWarning($"MainMenuController: {buttonName} button is not interactable! Enabling it...");
            button.interactable = true;
        }

        // Ensure button GameObject is active
        if (!button.gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"MainMenuController: {buttonName} button GameObject is inactive! Activating it...");
            button.gameObject.SetActive(true);
        }

        // remove existing listeners to avoid duplicates
        button.onClick.RemoveAllListeners();

        // add listener
        button.onClick.AddListener(action);
        button.onClick.AddListener(PlayButtonSound);
        Debug.Log($"MainMenuController: {buttonName} button listener added successfully. Button is interactable: {button.interactable}, Active: {button.gameObject.activeInHierarchy}");
    }

    void PlayButtonSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }

    public void OnStartLevel1Clicked()
    {
        Debug.Log("MainMenuController: OnStartLevel1Clicked called!");
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(level1SceneName);
        }
        else
        {
            SceneManager.LoadScene(level1SceneName);
        }
    }

    public void OnStartLevel2Clicked()
    {
        Debug.Log("MainMenuController: OnStartLevel2Clicked called!");
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(level2SceneName);
        }
        else
        {
            SceneManager.LoadScene(level2SceneName);
        }
    }

    public void OnTutorialClicked()
    {
        Debug.Log("MainMenuController: OnTutorialClicked called!");
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(tutorialSceneName);
        }
        else
        {
            SceneManager.LoadScene(tutorialSceneName);
        }
    }

    public void OnOptionsClicked()
    {
        Debug.Log("MainMenuController: OnOptionsClicked called!");
        if (optionsMenuPanel != null)
        {
            optionsMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("MainMenuController: Options menu panel is not assigned!");
        }
    }

    public void OnLoadGameClicked()
    {
        Debug.Log("MainMenuController: OnLoadGameClicked called!");
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSaveFile())
        {
            SaveSystem.Instance.LoadGame();
        }
        else
        {
            Debug.LogWarning("MainMenuController: No save file found.");
        }
    }

    public void OnQuitClicked()
    {
        Debug.Log("MainMenuController: OnQuitClicked called!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void OnDestroy()
    {
        if (startLevel1Button != null)
        {
            startLevel1Button.onClick.RemoveAllListeners();
        }

        if (startLevel2Button != null)
        {
            startLevel2Button.onClick.RemoveAllListeners();
        }

        if (tutorialButton != null)
        {
            tutorialButton.onClick.RemoveAllListeners();
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.RemoveAllListeners();
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
        }

        if (loadGameButton != null)
        {
            loadGameButton.onClick.RemoveAllListeners();
        }
    }
}

