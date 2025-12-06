using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("Tutorial Screen")]
    [SerializeField] private GameObject tutorialScreen;

    [Header("Settings")]
    [SerializeField] private Button skipButton;

    private bool tutorialCompleted = false;

    void Start()
    {
        // Show tutorial screen initially
        if (tutorialScreen != null)
        {
            tutorialScreen.SetActive(true);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
        }
    }

    public void SkipTutorial()
    {
        // Hide tutorial screen
        if (tutorialScreen != null)
        {
            tutorialScreen.SetActive(false);
        }

        tutorialCompleted = true;

        // Start the game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }

    public bool IsTutorialCompleted()
    {
        return tutorialCompleted;
    }

    void OnDestroy()
    {
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
        }
    }
}

