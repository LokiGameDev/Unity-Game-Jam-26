using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-100)]
public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private AudioDatabase database;
    [SerializeField] private AudioSettings settings;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Music")]
    [SerializeField, Min(0f)] private float defaultMusicFadeDuration = 1.5f;

    private MusicController musicController;
    private SFXController sfxController;

    private const string MasterKey = "Audio_Master";
    private const string MusicKey = "Audio_Music";
    private const string SFXKey = "Audio_SFX";

    public float MasterVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicController = GetComponent<MusicController>();
        if (musicController == null)
            musicController = gameObject.AddComponent<MusicController>();

        sfxController = GetComponent<SFXController>();
        if (sfxController == null)
            sfxController = gameObject.AddComponent<SFXController>();

        musicController.Initialize(musicGroup);

        int initial = settings != null ? settings.initialSFXSources : 12;
        int maximum = settings != null ? settings.maxSFXSources : 32;
        sfxController.Initialize(sfxGroup, initial, maximum);

        LoadVolumes();
        ApplyAllVolumes();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // -------------------- MUSIC --------------------

    public void PlayMusic(string audioID)
    {
        PlayMusic(audioID, defaultMusicFadeDuration);
    }

    public void PlayMusic(string audioID, float fadeDuration)
    {
        AudioEntry entry = GetEntry(audioID, AudioCategory.Music);
        if (entry != null)
            musicController.Play(entry, fadeDuration);
    }

    public void StopMusic()
    {
        StopMusic(defaultMusicFadeDuration);
    }

    public void StopMusic(float fadeDuration)
    {
        musicController?.Stop(fadeDuration);
    }

    public void StopMusicImmediately()
    {
        musicController?.StopImmediately();
    }

    // -------------------- SFX --------------------

    public void PlaySFX(string audioID)
    {
        PlaySFX(audioID, Vector3.zero, false);
    }

    public void PlaySFXAt(string audioID, Vector3 worldPosition)
    {
        PlaySFX(audioID, worldPosition, true);
    }

    private void PlaySFX(string audioID, Vector3 position, bool worldPosition)
    {
        AudioEntry entry = GetEntry(audioID, AudioCategory.SFX);
        if (entry != null)
            sfxController.Play(entry, position, worldPosition);
    }

    // For looping sounds, keep the returned AudioSource and call Stop() when finished.
    public AudioSource PlayLoopSFX(string audioID)
    {
        AudioEntry entry = GetEntry(audioID, AudioCategory.SFX);
        if (entry == null)
            return null;

        if (!entry.Loop)
            Debug.LogWarning($"'{audioID}' is not configured to loop.", entry);

        return sfxController.Play(entry, Vector3.zero, false);
    }

    public void StopAllSFX()
    {
        sfxController?.StopAll();
    }

    // -------------------- VOLUME --------------------

    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);
        ApplyVolume(settings != null ? settings.masterVolumeParameter : "MasterVolume", MasterVolume);
        SaveVolumes();
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        ApplyVolume(settings != null ? settings.musicVolumeParameter : "MusicVolume", MusicVolume);
        SaveVolumes();
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);
        ApplyVolume(settings != null ? settings.sfxVolumeParameter : "SFXVolume", SFXVolume);
        SaveVolumes();
    }

    private void ApplyAllVolumes()
    {
        ApplyVolume(settings != null ? settings.masterVolumeParameter : "MasterVolume", MasterVolume);
        ApplyVolume(settings != null ? settings.musicVolumeParameter : "MusicVolume", MusicVolume);
        ApplyVolume(settings != null ? settings.sfxVolumeParameter : "SFXVolume", SFXVolume);
    }

    private void ApplyVolume(string parameter, float normalizedValue)
    {
        if (settings == null || settings.mixer == null || string.IsNullOrWhiteSpace(parameter))
            return;

        // AudioMixer uses a logarithmic dB scale. -80 dB is effectively silent.
        float decibels = normalizedValue <= 0.0001f
            ? -80f
            : Mathf.Log10(normalizedValue) * 20f;

        if (!settings.mixer.SetFloat(parameter, decibels))
            Debug.LogWarning($"Audio Mixer parameter '{parameter}' is not exposed or was not found.");
    }

    private void LoadVolumes()
    {
        float masterDefault = settings != null ? settings.defaultMasterVolume : 1f;
        float musicDefault = settings != null ? settings.defaultMusicVolume : 0.8f;
        float sfxDefault = settings != null ? settings.defaultSFXVolume : 1f;

        bool loadSaved = settings != null && settings.saveVolumeSettings;

        MasterVolume = loadSaved ? PlayerPrefs.GetFloat(MasterKey, masterDefault) : masterDefault;
        MusicVolume = loadSaved ? PlayerPrefs.GetFloat(MusicKey, musicDefault) : musicDefault;
        SFXVolume = loadSaved ? PlayerPrefs.GetFloat(SFXKey, sfxDefault) : sfxDefault;
    }

    private void SaveVolumes()
    {
        if (settings == null || !settings.saveVolumeSettings)
            return;

        PlayerPrefs.SetFloat(MasterKey, MasterVolume);
        PlayerPrefs.SetFloat(MusicKey, MusicVolume);
        PlayerPrefs.SetFloat(SFXKey, SFXVolume);
        PlayerPrefs.Save();
    }

    // -------------------- DATABASE --------------------

    private AudioEntry GetEntry(string audioID, AudioCategory expectedCategory)
    {
        if (database == null)
        {
            Debug.LogError("[AudioManager] Audio Database is not assigned.", this);
            return null;
        }

        AudioEntry entry = database.Get(audioID);
        if (entry == null)
        {
            Debug.LogWarning($"[AudioManager] Audio ID '{audioID}' was not found.", this);
            return null;
        }

        if (entry.Category != expectedCategory)
        {
            Debug.LogWarning(
                $"[AudioManager] '{audioID}' is {entry.Category}, but {expectedCategory} was requested.",
                entry
            );
            return null;
        }

        return entry;
    }
}
