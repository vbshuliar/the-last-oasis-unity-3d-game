using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// handles tutorial screen buttons for starting levels or returning to main menu
public class TutorialController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button startLevel1Button;
    [SerializeField] private Button startLevel2Button;
    [SerializeField] private Button mainMenuButton;

    [Header("Scene Names")]
    [SerializeField] private string level1SceneName = "Level1";
    [SerializeField] private string level2SceneName = "Level2";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        // setup button listeners
        SetupButton(startLevel1Button, OnStartLevel1Clicked, "Start Level 1");
        SetupButton(startLevel2Button, OnStartLevel2Clicked, "Start Level 2");
        SetupButton(mainMenuButton, OnMainMenuClicked, "Main Menu");

        Time.timeScale = 1f;
    }

    void SetupButton(Button button, UnityEngine.Events.UnityAction action, string buttonName)
    {
        if (button == null)
        {
            Debug.LogWarning($"TutorialController: {buttonName} button is not assigned!");
            return;
        }

        // ensure button is interactable
        if (!button.interactable)
        {
            Debug.LogWarning($"TutorialController: {buttonName} button is not interactable! Enabling it...");
            button.interactable = true;
        }

        // remove existing listeners to avoid duplicates
        button.onClick.RemoveAllListeners();

        // add listener
        button.onClick.AddListener(action);
        button.onClick.AddListener(PlayButtonSound);
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
        Debug.Log("TutorialController: OnStartLevel1Clicked called!");
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
        Debug.Log("TutorialController: OnStartLevel2Clicked called!");
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(level2SceneName);
        }
        else
        {
            SceneManager.LoadScene(level2SceneName);
        }
    }

    public void OnMainMenuClicked()
    {
        Debug.Log("TutorialController: OnMainMenuClicked called!");
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    void OnDestroy()
    {
        // clean up button listeners
        if (startLevel1Button != null)
        {
            startLevel1Button.onClick.RemoveAllListeners();
        }

        if (startLevel2Button != null)
        {
            startLevel2Button.onClick.RemoveAllListeners();
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
        }
    }
}

