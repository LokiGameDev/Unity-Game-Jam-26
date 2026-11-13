
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SequentialParagraphTyper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text[] paragraphs;
    [SerializeField] private TMP_Text signature;

    [Header("Typing Settings")]
    [SerializeField] private float charactersPerSecond = 35f;
    [SerializeField] private float paragraphDelay = 0.8f;
    [SerializeField] private float signatureDelay = 1f;

    [Header("Events")]
    public UnityEvent onTypingStarted;
    public UnityEvent onParagraphCompleted;
    public UnityEvent onTypingCompleted;

    private Coroutine typingCoroutine;
    private int currentParagraph;

    private bool isTyping;
    private bool isCompleted;

    public bool IsTyping => isTyping;
    public bool IsCompleted => isCompleted;

    private void Start()
    {
        PrepareText();
    }

    private void PrepareText()
    {
        foreach (TMP_Text paragraph in paragraphs)
        {
            paragraph.maxVisibleCharacters = 0;
        }

        if (signature != null)
            signature.maxVisibleCharacters = 0;
    }

    public void StartTyping()
    {
        if (isTyping || isCompleted)
            return;

        typingCoroutine = StartCoroutine(TypeAllParagraphs());
    }

    private IEnumerator TypeAllParagraphs()
    {
        isTyping = true;

        onTypingStarted?.Invoke();

        for (int i = 0; i < paragraphs.Length; i++)
        {
            currentParagraph = i;

            TMP_Text currentText = paragraphs[i];

            currentText.gameObject.SetActive(true);

            currentText.ForceMeshUpdate();

            int totalCharacters =
                currentText.textInfo.characterCount;

            float delay = 1f / charactersPerSecond;

            for (int j = 0; j <= totalCharacters; j++)
            {
                currentText.maxVisibleCharacters = j;

                yield return new WaitForSeconds(delay);
            }

            currentText.maxVisibleCharacters = totalCharacters;

            onParagraphCompleted?.Invoke();

            if (i < paragraphs.Length - 1)
                yield return new WaitForSeconds(paragraphDelay);
        }

        // Reveal signature
        if (signature != null)
        {
            yield return new WaitForSeconds(signatureDelay);

            signature.gameObject.SetActive(true);
            signature.ForceMeshUpdate();

            int totalCharacters =
                signature.textInfo.characterCount;

            float delay = 1f / charactersPerSecond;

            for (int i = 0; i <= totalCharacters; i++)
            {
                signature.maxVisibleCharacters = i;
                yield return new WaitForSeconds(delay);
            }
        }

        isTyping = false;
        isCompleted = true;

        typingCoroutine = null;

        onTypingCompleted?.Invoke();
    }

    // Immediately reveal the remaining text
    public void SkipTyping()
    {
        if (!isTyping)
            return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        foreach (TMP_Text paragraph in paragraphs)
        {
            paragraph.maxVisibleCharacters = int.MaxValue;
        }

        if (signature != null)
            signature.maxVisibleCharacters = int.MaxValue;

        isTyping = false;
        isCompleted = true;

        typingCoroutine = null;

        onTypingCompleted?.Invoke();
    }

    public void ResetTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = false;
        isCompleted = false;

        currentParagraph = 0;

        PrepareText();
    }
}
