using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryEndingManager : MonoBehaviour
{
    [SerializeField] private SequentialParagraphTyper sequentialParagraphTyper;
    private void Start()
    {
        StartCoroutine(StartTheStory());
    }

    private IEnumerator StartTheStory()
    {
        yield return new WaitForSeconds(0.5f);
        sequentialParagraphTyper.StartTyping();
    }

    public void EndTheStory()
    {
        SceneManager.LoadScene(0);
    }
}
