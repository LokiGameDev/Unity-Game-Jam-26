using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Game/Audio/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    [SerializeField] private AudioEntry[] entries;

    private Dictionary<string, AudioEntry> lookup;

    private void OnEnable()
    {
        lookup = null;
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<string, AudioEntry>();

        if (entries == null)
            return;

        foreach (AudioEntry entry in entries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.AudioID))
                continue;

            if (lookup.ContainsKey(entry.AudioID))
            {
                Debug.LogWarning($"[AudioDatabase] Duplicate audio ID '{entry.AudioID}' in {name}.", this);
                continue;
            }

            lookup.Add(entry.AudioID, entry);
        }
    }

    public AudioEntry Get(string audioID)
    {
        if (string.IsNullOrWhiteSpace(audioID))
            return null;

        if (lookup == null)
            BuildLookup();

        lookup.TryGetValue(audioID, out AudioEntry entry);
        return entry;
    }

    public bool TryGet(string audioID, out AudioEntry entry)
    {
        entry = Get(audioID);
        return entry != null;
    }
}
