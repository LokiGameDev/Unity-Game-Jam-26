using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public sealed class MusicController : MonoBehaviour
{
    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource activeSource;
    private Coroutine transitionCoroutine;
    private string currentMusicID;

    public string CurrentMusicID => currentMusicID;
    public bool IsPlaying => activeSource != null && activeSource.isPlaying;

    public void Initialize(AudioMixerGroup outputGroup)
    {
        sourceA = CreateSource("Music A", outputGroup);
        sourceB = CreateSource("Music B", outputGroup);
        activeSource = sourceA;
    }

    private AudioSource CreateSource(string sourceName, AudioMixerGroup group)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = group;
        source.volume = 0f;
        source.name = sourceName;
        return source;
    }

    public void Play(AudioEntry entry, float fadeDuration)
    {
        if (entry == null || entry.Category != AudioCategory.Music)
            return;

        AudioClip clip = entry.GetRandomClip();
        if (clip == null)
        {
            Debug.LogWarning($"[MusicController] No clips assigned to '{entry.name}'.", entry);
            return;
        }

        if (currentMusicID == entry.AudioID &&
            activeSource != null &&
            activeSource.clip == clip &&
            activeSource.isPlaying)
            return;

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        AudioSource nextSource = activeSource == sourceA ? sourceB : sourceA;

        nextSource.Stop();
        nextSource.clip = clip;
        nextSource.loop = entry.Loop;
        nextSource.pitch = entry.GetPitch();
        nextSource.volume = 0f;
        nextSource.Play();

        AudioSource previousSource = activeSource;
        activeSource = nextSource;
        currentMusicID = entry.AudioID;

        transitionCoroutine = StartCoroutine(
            CrossFade(previousSource, nextSource, entry.Volume, Mathf.Max(0f, fadeDuration))
        );
    }

    public void Stop(float fadeDuration)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(
            FadeOutAndStop(activeSource, Mathf.Max(0f, fadeDuration))
        );
        currentMusicID = null;
    }

    public void SetMusicVolume(float volume)
    {
        // Per-track volume is applied during playback. Mixer volume is controlled by AudioManager.
        if (activeSource != null && activeSource.isPlaying)
            activeSource.volume = Mathf.Clamp01(volume);
    }

    private IEnumerator CrossFade(AudioSource oldSource, AudioSource newSource, float targetVolume, float duration)
    {
        float oldStart = oldSource != null ? oldSource.volume : 0f;

        if (duration <= 0f)
        {
            if (oldSource != null)
            {
                oldSource.Stop();
                oldSource.volume = 0f;
            }

            newSource.volume = targetVolume;
            transitionCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (oldSource != null)
                oldSource.volume = Mathf.Lerp(oldStart, 0f, t);

            newSource.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        if (oldSource != null)
        {
            oldSource.Stop();
            oldSource.volume = 0f;
        }

        newSource.volume = targetVolume;
        transitionCoroutine = null;
    }

    private IEnumerator FadeOutAndStop(AudioSource source, float duration)
    {
        if (source == null)
            yield break;

        float startVolume = source.volume;

        if (duration <= 0f)
        {
            source.Stop();
            source.volume = 0f;
            transitionCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        source.Stop();
        source.volume = 0f;
        transitionCoroutine = null;
    }

    public void StopImmediately()
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        sourceA?.Stop();
        sourceB?.Stop();

        if (sourceA != null) sourceA.volume = 0f;
        if (sourceB != null) sourceB.volume = 0f;

        currentMusicID = null;
        transitionCoroutine = null;
    }
}
