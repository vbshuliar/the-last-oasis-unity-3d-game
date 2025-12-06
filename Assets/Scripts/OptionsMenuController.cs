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

    private bool wasGamePaused = false;

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

    void Update()
    {
        // Handle Esc key to close options menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsMenuPanel != null && optionsMenuPanel.activeSelf)
            {
                OnBackClicked();
            }
        }
    }

    void OnEnable()
    {
        // Check if game was paused when options opened
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Paused)
        {
            wasGamePaused = true;
        }
    }

    void LoadSettings()
    {
        // load music volume
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
        }

        // load sfx volume
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
        }

        // load difficulty from playerprefs (with fallback to gamemanager if available)
        if (difficultyDropdown != null)
        {
            int savedDifficulty = 0;
            
            // try to load from playerprefs first
            if (PlayerPrefs.HasKey("Difficulty"))
            {
                savedDifficulty = PlayerPrefs.GetInt("Difficulty");
            }
            // fallback to gamemanager if playerprefs doesn't have it
            else if (GameManager.Instance != null)
            {
                savedDifficulty = (int)GameManager.Instance.GetDifficulty();
            }
            
            difficultyDropdown.value = savedDifficulty;
            
            // update gamemanager if it exists to keep them in sync
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

        // update audiomanager if it exists
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }

    void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();

        // update audiomanager if it exists
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }

    void OnDifficultyChanged(int value)
    {
        Difficulty difficulty = (Difficulty)value;
        
        // save directly to playerprefs to ensure persistence
        PlayerPrefs.SetInt("Difficulty", (int)difficulty);
        PlayerPrefs.Save();
        
        // update gamemanager if it exists to keep them in sync
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty(difficulty);
        }
    }

    public void OnBackClicked()
    {
        if (optionsMenuPanel != null)
        {
            optionsMenuPanel.SetActive(false);
            
            // if game was paused when options opened, resume it
            if (wasGamePaused && GameManager.Instance != null)
            {
                if (GameManager.Instance.CurrentState == GameState.Paused)
                {
                    GameManager.Instance.ResumeGame();
                }
                wasGamePaused = false;
            }
        }
        else
        {
            Debug.LogWarning("OptionsMenuController: Options menu panel is not assigned!");
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

