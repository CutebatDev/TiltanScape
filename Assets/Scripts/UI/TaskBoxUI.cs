using System.Collections.Generic;
using UnityEngine;

public class TaskBoxUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform taskList;
    [SerializeField] private TaskItemUI taskItemPrefab;

    private readonly Dictionary<string, TaskItemUI> activeTasks = new();

    public bool HasTask(string taskId)
    {
        return activeTasks.ContainsKey(taskId);
    }

    public TaskItemUI AddTask(string taskId, string taskDescription, float initialProgress = 0f)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            Debug.LogWarning("TaskBoxUI: Task ID cannot be null, empty, or whitespace.");
            return null;
        }

        if (taskList == null)
        {
            Debug.LogWarning("TaskBoxUI: TaskList wasn't assigned in the Inspector.");
            return null;
        }

        if (taskItemPrefab == null)
        {
            Debug.LogWarning("TaskBoxUI: TaskItemPrefab wasn't assigned in the Inspector.");
            return null;
        }

        if (activeTasks.ContainsKey(taskId))
        {
            Debug.LogWarning($"TaskBoxUI: A task with id '{taskId}' already exists.");
            return activeTasks[taskId];
        }

        TaskItemUI newTask = Instantiate(taskItemPrefab, taskList);
        newTask.gameObject.SetActive(true);
        newTask.Setup(taskId, taskDescription, initialProgress);

        activeTasks.Add(taskId, newTask);
        return newTask;
    }

    public void SetTaskDescription(string taskId, string description)
    {
        if (activeTasks.TryGetValue(taskId, out TaskItemUI task))
        {
            task.SetDescription(description);
        }
        else
        {
            Debug.LogWarning($"TaskBoxUI: Task '{taskId}' was not found.");
        }
    }

    public void SetTaskProgress(string taskId, float normalizedProgress)
    {
        if (activeTasks.TryGetValue(taskId, out TaskItemUI task))
        {
            task.SetProgress(normalizedProgress);
        }
        else
        {
            Debug.LogWarning($"TaskBoxUI: Task '{taskId}' was not found.");
        }
    }

    public void SetTaskProgressPercent(string taskId, float percent)
    {
        if (activeTasks.TryGetValue(taskId, out TaskItemUI task))
        {
            task.SetProgressPercent(percent);
        }
        else
        {
            Debug.LogWarning($"TaskBoxUI: Task '{taskId}' was not found.");
        }
    }

    public void RemoveTask(string taskId)
    {
        if (activeTasks.TryGetValue(taskId, out TaskItemUI task))
        {
            activeTasks.Remove(taskId);

            if (task != null)
                Destroy(task.gameObject);
        }
        else
        {
            Debug.LogWarning($"TaskBoxUI: Task '{taskId}' was not found.");
        }
    }

    public void ClearTasks()
    {
        foreach (TaskItemUI task in activeTasks.Values)
        {
            if (task != null)
                Destroy(task.gameObject);
        }

        activeTasks.Clear();
    }

    [ContextMenu("Add Test Task")]
    private void AddTestTask()
    {
        string id = "task_" + activeTasks.Count;
        AddTask(id, "New task added by UI event.", 0.35f);
    }

    [ContextMenu("Test Progress 75% On First Task")]
    private void TestProgress()
    {
        if (activeTasks.Count == 0)
        {
            Debug.LogWarning("TaskBoxUI: No tasks exist yet.");
            return;
        }

        foreach (TaskItemUI task in activeTasks.Values)
        {
            if (task != null)
            {
                task.SetProgressPercent(75f);
                break;
            }
        }
    }
}