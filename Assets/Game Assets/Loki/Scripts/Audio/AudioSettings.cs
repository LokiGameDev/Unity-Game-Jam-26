using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Game/Audio/Audio Settings")]
public class AudioSettings : ScriptableObject
{
    [Header("Mixer")]
    public AudioMixer mixer;

    [Tooltip("Exposed AudioMixer parameter names. Leave blank to skip mixer control.")]
    public string masterVolumeParameter = "MasterVolume";
    public string musicVolumeParameter = "MusicVolume";
    public string sfxVolumeParameter = "SFXVolume";

    [Header("Defaults")]
    [Range(0f, 1f)] public float defaultMasterVolume = 1f;
    [Range(0f, 1f)] public float defaultMusicVolume = 0.8f;
    [Range(0f, 1f)] public float defaultSFXVolume = 1f;

    [Header("SFX Pool")]
    [Min(1)] public int initialSFXSources = 12;
    [Min(1)] public int maxSFXSources = 32;

    [Header("Persistence")]
    public bool saveVolumeSettings = true;
}
