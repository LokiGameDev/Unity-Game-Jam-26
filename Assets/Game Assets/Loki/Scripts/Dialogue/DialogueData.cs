using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [Tooltip("Name displayed above the dialogue.")]
    public string speakerName;

    [Tooltip("Optional character portrait for this line.")]
    public Sprite portrait;

    [TextArea(2, 5)]
    public string text;
}

[CreateAssetMenu(fileName = "Dialogue_", menuName = "The Ninth Hour/Narrative/Dialogue")]
public class DialogueData : ScriptableObject
{
    [Tooltip("Unique ID for this conversation.")]
    public string dialogueID;

    public DialogueLine[] lines;
}
