using UnityEngine;

public class QuestTrackerUIManager : MonoBehaviour
{
    [SerializeField] private TaskBoxUI taskBoxUI;

    void OnEnable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStarted += HandleQuestStarted;
            QuestManager.Instance.OnQuestProgressUpdated += HandleQuestProgress;
            QuestManager.Instance.OnQuestTurnedIn += HandleQuestTurnedIn;
        }    
    }

    void OnDisable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStarted -= HandleQuestStarted;
            QuestManager.Instance.OnQuestProgressUpdated -= HandleQuestProgress;
            QuestManager.Instance.OnQuestTurnedIn -= HandleQuestTurnedIn;
        }    
    }

    void Start()
    {
        if (QuestManager.Instance == null) return;

        foreach (var quest in QuestManager.Instance.GetActiveQuests())
        {
            taskBoxUI.AddTask(quest.Data.Id, quest.Data.Title, quest.Progress / 100);
        }
    }

    private void HandleQuestStarted(Quest quest)
    {
        taskBoxUI.AddTask(quest.Data.Id, quest.Data.Title, 0f);
    }

    private void HandleQuestProgress(Quest quest)
    {
        float normalized = quest.Progress / 100;
        taskBoxUI.SetTaskProgress(quest.Data.Id, normalized);
    }

    private void HandleQuestTurnedIn(Quest quest)
    {
        taskBoxUI.RemoveTask(quest.Data.Id);
    }
}
