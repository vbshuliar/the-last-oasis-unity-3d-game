using UnityEngine;

// minimal audio controller that plays music and a few sfx clips
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] AudioClip punchClip;
    [SerializeField] AudioClip coinClip;
    [SerializeField] AudioClip potionClip;
    [SerializeField] AudioClip buttonClip;

    [Header("Volumes")]
    [Range(0f, 1f)][SerializeField] float musicVolume = 0.2f;
    [Range(0f, 1f)][SerializeField] float sfxVolume = 1.0f;

    // builds the singleton instance and prepares audio sources
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureAudioSources();
        ApplyVolumes();
    }

    // begins looping the background track at startup
    void Start()
    {
        PlayBackgroundMusic();
    }

    // makes sure dedicated music and sfx sources exist
    void EnsureAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = CreateChildSource("MusicSource", true);
        }

        if (sfxSource == null)
        {
            sfxSource = CreateChildSource("SFXSource", false);
        }
    }

    // spawns a child gameobject with an audio source attached
    AudioSource CreateChildSource(string name, bool loop)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(transform);
        AudioSource source = child.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = loop;
        return source;
    }

    // applies the current persisted volume levels to sources
    void ApplyVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    // starts playing the configured background music clip
    public void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusic == null)
        {
            return;
        }

        musicSource.clip = backgroundMusic;
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    // updates the music volume and clamps it to valid range
    public void SetMusicVolume(float normalizedVolume)
    {
        musicVolume = Mathf.Clamp01(normalizedVolume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    // updates the sfx volume and clamps it to valid range
    public void SetSFXVolume(float normalizedVolume)
    {
        sfxVolume = Mathf.Clamp01(normalizedVolume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    // plays a positional punch sound at the supplied location
    public void PlayPunchSound(Vector3 position)
    {
        PlaySpatialClip(punchClip, position);
    }

    // plays the coin pickup ui clip
    public void PlayCoinPickupSound()
    {
        PlayUISfx(coinClip);
    }

    // plays the potion pickup ui clip
    public void PlayPotionPickupSound()
    {
        PlayUISfx(potionClip);
    }

    // selects a clip based on which pickup was collected
    public void PlayPickupSoundForItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Star:
                PlayCoinPickupSound();
                break;
            case ItemType.SpeedBoost:
            case ItemType.DamageBoost:
            case ItemType.HealthPack:
                PlayPotionPickupSound();
                break;
            default:
                PlayPotionPickupSound();
                break;
        }
    }

    // plays a ui friendly sound for button presses
    public void PlayButtonClickSound()
    {
        if (buttonClip != null)
        {
            PlayUISfx(buttonClip);
        }
        else if (coinClip != null || potionClip != null)
        {
            PlayUISfx(coinClip != null ? coinClip : potionClip);
        }
    }

    // plays a one shot at the given world position
    void PlaySpatialClip(AudioClip clip, Vector3 position)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
    }

    // triggers a one shot on the shared ui sfx source
    void PlayUISfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}

