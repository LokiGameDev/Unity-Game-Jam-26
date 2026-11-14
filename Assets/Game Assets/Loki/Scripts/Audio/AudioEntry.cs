using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioEntry", menuName = "Game/Audio/Audio Entry")]
public class AudioEntry : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private AudioID audioID;
    [SerializeField] private AudioCategory category;

    [Header("Clips")]
    [SerializeField] private AudioClip[] clips;

    [Header("Playback")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;
    [SerializeField] private bool loop;

    [Header("Pitch")]
    [SerializeField] private bool randomizePitch;
    [SerializeField, Min(0.01f)] private float minPitch = 0.9f;
    [SerializeField, Min(0.01f)] private float maxPitch = 1.1f;

    [Header("3D Audio")]
    [Range(0f, 1f)]
    [SerializeField] private float spatialBlend;
    [SerializeField, Min(0f)] private float minDistance = 1f;
    [SerializeField, Min(0f)] private float maxDistance = 25f;

    public string AudioID => audioID.ToString();
    public AudioCategory Category => category;
    public bool Loop => loop;
    public float Volume => volume;
    public float SpatialBlend => spatialBlend;
    public float MinDistance => minDistance;
    public float MaxDistance => Mathf.Max(minDistance, maxDistance);

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0)
            return null;

        return clips[Random.Range(0, clips.Length)];
    }

    public float GetPitch()
    {
        if (!randomizePitch)
            return 1f;

        float min = Mathf.Min(minPitch, maxPitch);
        float max = Mathf.Max(minPitch, maxPitch);
        return Random.Range(min, max);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        minDistance = Mathf.Max(0f, minDistance);
        maxDistance = Mathf.Max(minDistance, maxDistance);
        minPitch = Mathf.Max(0.01f, minPitch);
        maxPitch = Mathf.Max(0.01f, maxPitch);
    }
#endif
}
