using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class OptionsMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject optionsMenuPanel;

    [Header("Settings")]
    [SerializeField] private float defaultMusicVolume = 0.7f;
    [SerializeField] private float defaultSFXVolume = 0.8f;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        LoadSettings();

        // Setup listeners
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        if (difficultyDropdown != null)
        {
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    void LoadSettings()
    {
        // Load music volume
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
        }

        // Load SFX volume
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
        }

        // Load difficulty from PlayerPrefs (with fallback to GameManager if available)
        if (difficultyDropdown != null)
        {
            int savedDifficulty = 0;
            
            // Try to load from PlayerPrefs first
            if (PlayerPrefs.HasKey("Difficulty"))
            {
                savedDifficulty = PlayerPrefs.GetInt("Difficulty");
            }
            // Fallback to GameManager if PlayerPrefs doesn't have it
            else if (GameManager.Instance != null)
            {
                savedDifficulty = (int)GameManager.Instance.GetDifficulty();
            }
            
            difficultyDropdown.value = savedDifficulty;
            
            // Update GameManager if it exists to keep them in sync
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetDifficulty((Difficulty)savedDifficulty);
            }
        }
    }

    void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();

        // Update AudioManager if it exists
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }

    void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();

        // Update AudioManager if it exists
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }

    void OnDifficultyChanged(int value)
    {
        Difficulty difficulty = (Difficulty)value;
        
        // Save directly to PlayerPrefs to ensure persistence
        PlayerPrefs.SetInt("Difficulty", (int)difficulty);
        PlayerPrefs.Save();
        
        // Update GameManager if it exists to keep them in sync
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty(difficulty);
        }
    }

    public void OnBackClicked()
    {
        // If options menu panel exists in the same scene, hide it
        if (optionsMenuPanel != null)
        {
            optionsMenuPanel.SetActive(false);
        }
        // Otherwise, load the main menu scene
        else if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadScene(mainMenuSceneName);
            }
            else
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
        }
    }

    void OnDestroy()
    {
        // Clean up listeners
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        }

        if (difficultyDropdown != null)
        {
            difficultyDropdown.onValueChanged.RemoveAllListeners();
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
        }
    }
}

