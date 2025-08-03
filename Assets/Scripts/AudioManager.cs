using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource backgroundMusic;
    public AudioSource sfxSource;

    [Header("Master Volume")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ApplyVolume();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = masterVolume;
            if (!backgroundMusic.isPlaying && backgroundMusic.clip != null)
                backgroundMusic.Play();
        }
        if (sfxSource != null)
        {
            sfxSource.volume = masterVolume;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (backgroundMusic == null || clip == null) return;
        backgroundMusic.Stop();
        backgroundMusic.clip = clip;
        backgroundMusic.Play();
        backgroundMusic.volume = masterVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
