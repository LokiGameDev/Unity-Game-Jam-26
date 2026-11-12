using TMPro;
using UnityEngine;

public class TaskUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private GameObject objectiveRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    private void OnEnable()
    {
        if (taskManager == null)
            taskManager = TaskManager.Instance;

        if (taskManager != null)
        {
            taskManager.OnObjectiveChanged += UpdateObjective;
            UpdateObjective(taskManager.CurrentTask);
        }
        else
        {
            Debug.LogWarning("TaskUI could not find a TaskManager.", this);
        }
    }

    private void OnDisable()
    {
        if (taskManager != null)
            taskManager.OnObjectiveChanged -= UpdateObjective;
    }

    private void UpdateObjective(TaskData task)
    {
        bool hasTask = task != null;

        if (objectiveRoot != null)
            objectiveRoot.SetActive(hasTask);

        if (!hasTask)
            return;

        if (titleText != null)
            titleText.text = task.title;

        if (descriptionText != null)
            descriptionText.text = task.description;
    }
}
