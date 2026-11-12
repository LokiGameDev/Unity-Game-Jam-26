using System;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    public TaskData CurrentTask { get; private set; }

    public event Action<TaskData> OnObjectiveChanged;
    public event Action<TaskData> OnObjectiveCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one TaskManager exists. Destroying duplicate.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetObjective(TaskData task)
    {
        CurrentTask = task;
        OnObjectiveChanged?.Invoke(CurrentTask);
    }

    public void CompleteObjective(string taskID)
    {
        if (CurrentTask == null)
        {
            Debug.LogWarning($"Cannot complete '{taskID}': there is no active objective.", this);
            return;
        }

        if (CurrentTask.taskID != taskID)
        {
            Debug.LogWarning(
                $"Objective ID mismatch. Active: '{CurrentTask.taskID}', requested: '{taskID}'.",
                this
            );
            return;
        }

        TaskData completedTask = CurrentTask;
        CurrentTask = null;

        OnObjectiveCompleted?.Invoke(completedTask);
        OnObjectiveChanged?.Invoke(null);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
