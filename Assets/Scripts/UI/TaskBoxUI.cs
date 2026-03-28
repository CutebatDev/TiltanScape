using TMPro;
using UnityEngine;

public class TaskBoxUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform taskList;
    [SerializeField] private TMP_Text taskTextPrefab;


    public void AddTask(string taskDescription)
    {
        if (taskList == null)
        {
            Debug.LogWarning("TaskBoxUI: TaskList wasn't assigned in the Inspector.");
            return;
        }

        if (taskTextPrefab == null)
        {
            Debug.LogWarning("TaskBoxUI: TaskTextPrefab wasn't assigned in the Inspector.");
            return;
        }

        TMP_Text newTask = Instantiate(taskTextPrefab, taskList);
        newTask.text = taskDescription;
        newTask.gameObject.SetActive(true);
    }

    public void ClearTasks()
    {
        if (taskList == null) return;

        for (int i = taskList.childCount - 1; i >= 0; i--)
        {
            Destroy(taskList.GetChild(i).gameObject);
        }
    }


    [ContextMenu("Add Test Task")]
    private void AddTestTask()
    {
        AddTask("New task added by UI event.");
    }
}