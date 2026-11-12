using UnityEngine;

[CreateAssetMenu(fileName = "Task_", menuName = "The Ninth Hour/Narrative/Task")]
public class TaskData : ScriptableObject
{
    [Tooltip("Unique ID used by code to complete this objective.")]
    public string taskID;

    public string title;

    [TextArea(2, 4)]
    public string description;
}
