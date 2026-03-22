using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private Dictionary<string, Quest> activeQuests = new();
    private HashSet<string> completedQuests = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Critical to keep this quest manager persistent between school floors
        DontDestroyOnLoad(gameObject);
    }

    public Quest StartQuest(QuestData data)
    {
        if (activeQuests.ContainsKey(data.Id))
            return activeQuests[data.Id];

        var quest = new Quest(data);
        activeQuests.Add(data.Id, quest);
        return quest;
    }

    public void AddProgress(string questId, float amount)
    {
        if (!activeQuests.TryGetValue(questId, out var quest))
            return;

        quest.AddProgress(amount);
    }

    public void TurnInQuest(string questId)
    {
        if (!activeQuests.TryGetValue(questId, out var quest))
            return;

        if (!quest.IsCompleted)
            return;

        quest.TurnIn();
        activeQuests.Remove(questId);
        completedQuests.Add(questId);
    }

    public Quest GetActiveQuest(string questId)
    {
        activeQuests.TryGetValue(questId, out var quest);
        return quest;
    }

    public bool IsQuestCompleted(string questId) => completedQuests.Contains(questId);
}
