using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryBeginningManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
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
        yield return new WaitForSeconds(6.5f);
        sequentialParagraphTyper.StartTyping();
        typingEffect = AudioManager.Instance.PlayLoopSFX(AudioID.Typewriting);
    }

    public void StopTypewititingEffect()
    {
        typingEffect.Stop();
    }

    public void EndTheStory()
    {
        animator.SetTrigger("End");
        StartCoroutine(EndStartStory());
        
    }

    private IEnumerator EndStartStory()
    {
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.StopMusicImmediately();
        AudioManager.Instance.PlayMusic(AudioID.BGM);
        SceneManager.LoadScene("Main");
    }
}
