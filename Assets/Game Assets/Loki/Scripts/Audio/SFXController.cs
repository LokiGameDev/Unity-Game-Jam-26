using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public sealed class SFXController : MonoBehaviour
{
    private readonly List<AudioSource> sources = new List<AudioSource>();
    private AudioMixerGroup outputGroup;
    private int maxSources;

    public void Initialize(AudioMixerGroup group, int initialSources, int maximumSources)
    {
        outputGroup = group;
        maxSources = Mathf.Max(1, maximumSources);

        int count = Mathf.Clamp(initialSources, 1, maxSources);
        for (int i = 0; i < count; i++)
            CreateSource();
    }

    private AudioSource CreateSource()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = outputGroup;
        source.name = $"SFX Source {sources.Count + 1}";
        sources.Add(source);
        return source;
    }

    private AudioSource GetAvailableSource()
    {
        for (int i = 0; i < sources.Count; i++)
        {
            if (!sources[i].isPlaying)
                return sources[i];
        }

        if (sources.Count < maxSources)
            return CreateSource();

        // If the pool is full, reuse the source that has been playing longest.
        // This avoids creating an unlimited number of AudioSources.
        AudioSource oldest = sources[0];
        for (int i = 1; i < sources.Count; i++)
        {
            if (sources[i].time > oldest.time)
                oldest = sources[i];
        }

        oldest.Stop();
        return oldest;
    }

    public AudioSource Play(AudioEntry entry, Vector3 position, bool useWorldPosition)
    {
        if (entry == null || entry.Category != AudioCategory.SFX)
            return null;

        AudioClip clip = entry.GetRandomClip();
        if (clip == null)
        {
            Debug.LogWarning($"[SFXController] No clips assigned to '{entry.name}'.", entry);
            return null;
        }

        AudioSource source = GetAvailableSource();
        source.transform.position = position;
        source.clip = clip;
        source.volume = entry.Volume;
        source.pitch = entry.GetPitch();
        source.loop = entry.Loop;
        source.spatialBlend = useWorldPosition ? entry.SpatialBlend : 0f;
        source.minDistance = entry.MinDistance;
        source.maxDistance = entry.MaxDistance;
        source.Play();

        return source;
    }

    public void StopAll()
    {
        foreach (AudioSource source in sources)
            source.Stop();
    }
}
