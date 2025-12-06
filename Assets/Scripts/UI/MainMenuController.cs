using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartClicked);
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(OnOptionsClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        Time.timeScale = 1f;
    }

    public void OnStartClicked()
    {
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

