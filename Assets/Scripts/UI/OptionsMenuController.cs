using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// manages difficulty and audio settings within the options menu
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

    // loads previously saved settings and wires up callbacks
    void Start()
    {
        LoadSettings();

        // set up listeners
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
            backButton.onClick.AddListener(PlayButtonSound);
        }
    }

    // lets the escape key close the menu quickly
    void Update()
    {
        // handle esc key to close options menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsMenuPanel != null && optionsMenuPanel.activeSelf)
            {
                OnBackClicked();
            }
        }
    }

    // tracks whether the game was paused before opening the menu
    void OnEnable()
    {
        // check if game was paused when options opened
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Paused)
        {
            wasGamePaused = true;
        }
    }

    // populates the ui controls with saved values
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

        // load difficulty from player prefs (with fallback to gamemanager if available)
        if (difficultyDropdown != null)
        {
            int savedDifficulty = PlayerPrefs.GetInt("Difficulty", (int)Difficulty.Easy);
            difficultyDropdown.value = savedDifficulty;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetDifficulty((Difficulty)savedDifficulty);
            }
        }
    }

    // saves and applies the new music volume
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

    // saves and applies the new sfx volume
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

    // persists the selected difficulty and informs the game manager
    void OnDifficultyChanged(int value)
    {
        Difficulty difficulty = (Difficulty)value;

        PlayerPrefs.SetInt("Difficulty", (int)difficulty);
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty(difficulty);
        }
    }

    // hides the options panel and resumes the game if needed
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

    // plays a button click through the audio manager
    void PlayButtonSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }

    // removes listeners when the options menu is destroyed
    void OnDestroy()
    {
        // clean up listeners
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

