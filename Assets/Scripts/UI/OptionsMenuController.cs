using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Dropdown difficultyDropdown;

    [Header("Settings")]
    [SerializeField] private float defaultMusicVolume = 0.7f;
    [SerializeField] private float defaultSFXVolume = 0.8f;

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

        // Load difficulty
        if (difficultyDropdown != null && GameManager.Instance != null)
        {
            Difficulty currentDifficulty = GameManager.Instance.GetDifficulty();
            difficultyDropdown.value = (int)currentDifficulty;
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
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty(difficulty);
        }
    }

    public void OnBackClicked()
    {
        // This will be called by a back button
        // The actual navigation depends on your UI setup
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
    }
}

