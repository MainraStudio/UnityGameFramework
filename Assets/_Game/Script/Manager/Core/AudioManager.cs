using MainraFramework;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
using System.Collections;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header("Audio Sources")]
    [Tooltip("Audio source for background music")]
    public AudioSource bgmSource;

    [Tooltip("Audio source for sound effects")]
    public AudioSource sfxSource;

    [Header("Audio Mixer")]
    [Tooltip("Audio mixer for controlling audio levels")]
    public AudioMixer audioMixer;

    [Header("Audio Clip Library")]
    [Tooltip("Library of audio clips")]
    public AudioClipLibrary audioClipLibrary;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [Tooltip("Master volume level")]
    public float masterVolume = 1f;

    [Range(0f, 1f)]
    [Tooltip("Background music volume level")]
    public float bgmVolume = 1f;

    [Range(0f, 1f)]
    [Tooltip("Sound effects volume level")]
    public float sfxVolume = 1f;

    // Initialize AudioManager
    public void Initialize()
    {
        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        // Ensure AudioClipLibrary is connected
        if (audioClipLibrary == null)
        {
            Debug.LogError("AudioClipLibrary is not connected!");
            return;
        }

        // Set initial volume levels
        SetMasterVolume(masterVolume);
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
        Debug.Log("Audio Manager Initialized");
    }

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        if (audioMixer != null)
        {
            if (volume <= 0.01f)
                audioMixer.SetFloat(Parameter.AudioMixer.MasterVolumeParameter, -80f); // Set to -80 dB for silence
            else
                audioMixer.SetFloat(Parameter.AudioMixer.MasterVolumeParameter, Mathf.Log10(volume) * 20); // Convert to dB
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (audioMixer != null)
        {
            if (volume <= 0.01f)
                audioMixer.SetFloat(Parameter.AudioMixer.BGMVolumeParameter, -80f); // Set to -80 dB for silence
            else
                audioMixer.SetFloat(Parameter.AudioMixer.BGMVolumeParameter, Mathf.Log10(volume) * 20); // Convert to dB
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (audioMixer != null)
        {
            if (volume <= 0.01f)
                audioMixer.SetFloat(Parameter.AudioMixer.SFXVolumeParameter, -80f); // Set to -80 dB for silence
            else
                audioMixer.SetFloat(Parameter.AudioMixer.SFXVolumeParameter, Mathf.Log10(volume) * 20); // Convert to dB
        }
    }

    // Play background music (BGM)
    public void PlayBGM(string key, bool loop = true)
    {
        if (audioClipLibrary == null || bgmSource == null) return;

        var clip = audioClipLibrary.GetAudioClip(key, true); // Find BGM in library
        if (clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM with key '{key}' not found!");
        }
    }

    // Play sound effects (SFX)
    public void PlaySFX(string key)
    {
        if (audioClipLibrary == null || sfxSource == null) return;

        var clip = audioClipLibrary.GetAudioClip(key, false); // Find SFX in library
        if (clip != null)
            sfxSource.PlayOneShot(clip); // Play sound effect
        else
            Debug.LogWarning($"SFX with key '{key}' not found!");
    }

    // Stop background music
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    // Fade in audio
    public void FadeIn(AudioSource audioSource, float duration)
    {
        StartCoroutine(FadeInCoroutine(audioSource, duration));
    }

    private IEnumerator FadeInCoroutine(AudioSource audioSource, float duration)
    {
        float startVolume = 0f;
        audioSource.volume = startVolume;
        audioSource.Play();

        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / duration;
            yield return null;
        }

        audioSource.volume = 1f;
    }

    // Fade out audio
    public void FadeOut(AudioSource audioSource, float duration)
    {
        StartCoroutine(FadeOutCoroutine(audioSource, duration));
    }

    private IEnumerator FadeOutCoroutine(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    // Pause audio
    public void PauseAudio()
    {
        if (bgmSource != null) bgmSource.Pause();
        if (sfxSource != null) sfxSource.Pause();
    }

    // Resume audio
    public void ResumeAudio()
    {
        if (bgmSource != null) bgmSource.UnPause();
        if (sfxSource != null) sfxSource.UnPause();
    }

    // Mute audio
    public void MuteAudio()
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(Parameter.AudioMixer.MasterVolumeParameter, -80f);
        }
    }

    // Unmute audio
    public void UnmuteAudio()
    {
        SetMasterVolume(masterVolume);
    }

    // Save volume settings
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat(Parameter.PlayerPrefs.MASTER_VOLUME, masterVolume);
        PlayerPrefs.SetFloat(Parameter.PlayerPrefs.MUSIC_VOLUME, bgmVolume);
        PlayerPrefs.SetFloat(Parameter.PlayerPrefs.SFX_VOLUME, sfxVolume);
        PlayerPrefs.Save();
    }

    // Load volume settings
    public void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey(Parameter.PlayerPrefs.MASTER_VOLUME))
        {
            masterVolume = PlayerPrefs.GetFloat(Parameter.PlayerPrefs.MASTER_VOLUME);
            SetMasterVolume(masterVolume);
        }

        if (PlayerPrefs.HasKey(Parameter.PlayerPrefs.MUSIC_VOLUME))
        {
            bgmVolume = PlayerPrefs.GetFloat(Parameter.PlayerPrefs.MUSIC_VOLUME);
            SetBGMVolume(bgmVolume);
        }

        if (PlayerPrefs.HasKey(Parameter.PlayerPrefs.SFX_VOLUME))
        {
            sfxVolume = PlayerPrefs.GetFloat(Parameter.PlayerPrefs.SFX_VOLUME);
            SetSFXVolume(sfxVolume);
        }
    }
}