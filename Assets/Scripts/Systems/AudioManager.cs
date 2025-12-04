using UnityEngine;

// manages all game audio including music and sound effects
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip playerAttackSound;
    [SerializeField] private AudioClip enemyAttackSound;
    [SerializeField] private AudioClip itemPickupSound;
    [SerializeField] private AudioClip playerHurtSound;
    [SerializeField] private AudioClip enemyDeathSound;
    [SerializeField] private AudioClip footstepSound;

    [Header("Volume Settings")]
    [SerializeField] private float musicVolume = 0.7f;
    [SerializeField] private float sfxVolume = 0.8f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume); // clamp01 keeps value between 0 and 1
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, bool is3D = false)
    {
        if (clip == null) return;

        if (is3D)
        {
            // 3d sound gets quieter as you move away from position
            AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
        }
        else
        {
            // 2d sound plays at same volume regardless of position
            if (sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
        }
    }
    public void PlayPlayerAttackSound(Vector3 position)
    {
        PlaySound(playerAttackSound, position, true);
    }

    public void PlayEnemyAttackSound(Vector3 position)
    {
        PlaySound(enemyAttackSound, position, true);
    }

    public void PlayItemPickupSound()
    {
        PlaySound(itemPickupSound, Vector3.zero, false);
    }

    public void PlayPlayerHurtSound()
    {
        PlaySound(playerHurtSound, Vector3.zero, false);
    }

    public void PlayEnemyDeathSound(Vector3 position)
    {
        PlaySound(enemyDeathSound, position, true);
    }

    public void PlayFootstepSound(Vector3 position)
    {
        PlaySound(footstepSound, position, true);
    }
}

