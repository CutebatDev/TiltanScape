using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Events;
using Save_System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private Dictionary<string, Quest> activeQuests = new();
    private HashSet<string> completedQuests = new();

    // Events
    public delegate void QuestEvent(Quest quests);

    public event QuestEvent OnQuestStarted;
    public event QuestEvent OnQuestProgressUpdated;
    public event QuestEvent OnQuestProgressCompleted;
    public event QuestEvent OnQuestTurnedIn;

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

    private void Start()
    {
        EventsManager.Instance.OnGameSave += SaveQuests;
        EventsManager.Instance.OnGameLoad += LoadQuests;
    }

    public Quest StartQuest(QuestData data)
    {
        if (activeQuests.ContainsKey(data.Id))
            return activeQuests[data.Id];

        var quest = new Quest(data);
        activeQuests.Add(data.Id, quest);

        OnQuestStarted?.Invoke(quest);

        return quest;
    }

    public void AddProgress(string questId, float amount)
    {
        EventsManager.Instance.OnUseQuestStation.Invoke();
        if (!activeQuests.TryGetValue(questId, out var quest))
            return;

        bool wasCompleted = quest.IsCompleted;
        quest.AddProgress(amount);
        OnQuestProgressUpdated?.Invoke(quest);

        if (!wasCompleted && quest.IsCompleted)
            OnQuestProgressCompleted?.Invoke(quest);
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

        OnQuestTurnedIn?.Invoke(quest);
    }

    public Quest GetActiveQuest(string questId)
    {
        activeQuests.TryGetValue(questId, out var quest);
        return quest;
    }

    public List<Quest> GetActiveQuests() => activeQuests.Values.ToList();

    public bool IsQuestCompleted(string questId) => completedQuests.Contains(questId);
    public void GetActiveQuestNames() => Debug.Log(string.Join(", ", activeQuests.Values.Select(q => q.Data.Title)));

    public Save_System.PlayerQuestData GetQuestSaveData() => new(activeQuests, completedQuests);

    private void SaveQuests()
    {
        SaveSystem.SaveQuests();
    }

    private void LoadQuests()
    {
        PlayerQuestData data = SaveSystem.LoadQuests();
        if (data == null) return;

        activeQuests.Clear();
        completedQuests.Clear();

        if (data.activeQuestIds != null)
        {
            for (int i = 0; i < data.activeQuestIds.Count; i++)
            {
                string id = data.activeQuestIds[i];
                float progress = data.activeQuestProgress[i];

                QuestData questData = Resources.Load<QuestData>("Quests/" + id);
                if (questData != null)
                {
                    Quest quest = new Quest(questData);
                    quest.AddProgress(progress);
                    activeQuests[id] = quest;
                }
                else
                {
                    Debug.LogWarning($"Could not find QuestData for {id}");
                }
            }
        }

        if (data.completedQuestIds != null)
        {
            foreach (var id in data.completedQuestIds)
            {
                completedQuests.Add(id);
            }
        }
    }
}