using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryBeginningManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SequentialParagraphTyper sequentialParagraphTyper;
    private void Start()
    {
        StartCoroutine(StartTheStory());
    }

    private IEnumerator StartTheStory()
    {
        yield return new WaitForSeconds(6.5f);
        sequentialParagraphTyper.StartTyping();
    }

    public void EndTheStory()
    {
        animator.SetTrigger("End");
        StartCoroutine(EndStartStory());
        
    }

    private IEnumerator EndStartStory()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main");
    }
}
