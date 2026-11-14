using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryEndingManager : MonoBehaviour
{
    [SerializeField] private SequentialParagraphTyper sequentialParagraphTyper;

    private AudioSource typingEffect;
    private void Start()
    {
        AudioManager.Instance.StopMusicImmediately();
        AudioManager.Instance.PlayMusic(AudioID.StoryBGM);
        StartCoroutine(StartTheStory());
    }

    private IEnumerator StartTheStory()
    {
        yield return new WaitForSeconds(0.5f);
        sequentialParagraphTyper.StartTyping();
        typingEffect = AudioManager.Instance.PlayLoopSFX(AudioID.Typewriting);
    }

    public void StopTypewititingEffect()
    {
        typingEffect.Stop();
    }

    public void EndTheStory()
    {
        AudioManager.Instance.StopMusicImmediately();
        AudioManager.Instance.PlayMusic(AudioID.BGM);
        SceneManager.LoadScene(0);
    }
}
