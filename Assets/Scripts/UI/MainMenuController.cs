using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// handles main menu button clicks and scene loading
public class MainMenuController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [Header("Options Menu")]
    [SerializeField] private OptionsMenuController optionsMenuController;
    [SerializeField] private GameObject optionsMenuPanel;

    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "Main";
    [SerializeField] private string tutorialSceneName = "Tutorial";
    [SerializeField] private string optionsSceneName = "Options";

    void Start()
    {
        // Ensure EventSystem exists
        EnsureEventSystem();
        
        // Verify and setup buttons
        SetupButton(startButton, OnStartClicked, "Start");
        SetupButton(optionsButton, OnOptionsClicked, "Options");
        SetupButton(quitButton, OnQuitClicked, "Quit");

        Time.timeScale = 1f;
    }

    void EnsureEventSystem()
    {
        // Check if EventSystem exists in the scene
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

        // Check if Canvas has GraphicRaycaster
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

        // Remove existing listeners to avoid duplicates
        button.onClick.RemoveAllListeners();
        
        // Add listener
        button.onClick.AddListener(action);
        Debug.Log($"MainMenuController: {buttonName} button listener added successfully. Button is interactable: {button.interactable}, Active: {button.gameObject.activeInHierarchy}");
    }

    public void OnStartClicked()
    {
        Debug.Log("MainMenuController: OnStartClicked called!");
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(gameSceneName);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void OnTutorialClicked()
    {
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
        // If options menu panel exists in the same scene, show it
        if (optionsMenuPanel != null)
        {
            optionsMenuPanel.SetActive(true);
        }
        // Otherwise, load the options scene
        else if (!string.IsNullOrEmpty(optionsSceneName))
        {
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadScene(optionsSceneName);
            }
            else
            {
                SceneManager.LoadScene(optionsSceneName);
            }
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
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.RemoveAllListeners();
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
        }
    }
}

