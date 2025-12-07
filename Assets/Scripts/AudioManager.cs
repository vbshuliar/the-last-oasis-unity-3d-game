using UnityEngine;

// Minimal audio controller that only handles background music and three SFX (punch, coin, potion)
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
    [Range(0f, 1f)][SerializeField] float musicVolume = 0.7f;
    [Range(0f, 1f)][SerializeField] float sfxVolume = 0.9f;

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

    void Start()
    {
        PlayBackgroundMusic();
    }

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

    AudioSource CreateChildSource(string name, bool loop)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(transform);
        AudioSource source = child.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = loop;
        return source;
    }

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

    public void SetMusicVolume(float normalizedVolume)
    {
        musicVolume = Mathf.Clamp01(normalizedVolume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float normalizedVolume)
    {
        sfxVolume = Mathf.Clamp01(normalizedVolume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public void PlayPunchSound(Vector3 position)
    {
        PlaySpatialClip(punchClip, position);
    }

    public void PlayCoinPickupSound()
    {
        PlayUISfx(coinClip);
    }

    public void PlayPotionPickupSound()
    {
        PlayUISfx(potionClip);
    }

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

    void PlaySpatialClip(AudioClip clip, Vector3 position)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
    }

    void PlayUISfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}

