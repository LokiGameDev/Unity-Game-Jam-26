using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    [SerializeField] private AudioID audioID;

    public void PlaySoundEffects()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(audioID);
        }
    }
}
