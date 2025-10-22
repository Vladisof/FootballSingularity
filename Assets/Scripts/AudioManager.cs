using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip labMusic;
    public AudioClip mutationMusic;

    [Header("SFX Clips")]
    public AudioClip buttonClickSfx;
    public AudioClip successSfx;
    public AudioClip failureSfx;
    public AudioClip mutationStartSfx;
    public AudioClip mutationCompleteSfx;
    public AudioClip purchaseSfx;
    public AudioClip notificationSfx;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudio()
    {
        // Create audio sources if they don't exist
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        LoadVolumes();
    }

    private void LoadVolumes()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;
    }

    // Music Methods
    public void PlayMusic(AudioClip clip, bool fade = false)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        if (fade)
        {
            StartCoroutine(FadeMusic(clip));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuMusic, true);
    }

    public void PlayLabMusic()
    {
        PlayMusic(labMusic, true);
    }

    public void PlayMutationMusic()
    {
        PlayMusic(mutationMusic, true);
    }

    public void StopMusic(bool fade = false)
    {
        if (musicSource == null) return;

        if (fade)
        {
            StartCoroutine(FadeOutMusic());
        }
        else
        {
            musicSource.Stop();
        }
    }

    private System.Collections.IEnumerator FadeMusic(AudioClip newClip)
    {
        float fadeDuration = 1f;
        float startVolume = musicSource.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVolume;
    }

    private System.Collections.IEnumerator FadeOutMusic()
    {
        float fadeDuration = 1f;
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
    }

    // SFX Methods
    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void PlayButtonClick()
    {
        PlaySfx(buttonClickSfx);
    }

    public void PlaySuccess()
    {
        PlaySfx(successSfx);
    }

    public void PlayFailure()
    {
        PlaySfx(failureSfx);
    }

    public void PlayMutationStart()
    {
        PlaySfx(mutationStartSfx);
    }

    public void PlayMutationComplete()
    {
        PlaySfx(mutationCompleteSfx);
    }

    public void PlayPurchase()
    {
        PlaySfx(purchaseSfx);
    }

    public void PlayNotification()
    {
        PlaySfx(notificationSfx);
    }
}
