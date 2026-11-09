using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject textGameObject;

    private Coroutine coroutine;

    private void Start()
    {
        textGameObject.SetActive(false);
    }

    public void ShowDialogue(string msg)
    {
        textGameObject.SetActive(true);
        dialogueText.text = msg;

        if(coroutine!=null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(DisplayDelay());
    }

    private IEnumerator DisplayDelay()
    {
        yield return new WaitForSeconds(2);
        textGameObject.SetActive(false);
        coroutine = null;
    }
}
