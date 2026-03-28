using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ActionInteractable))]
public class NPCScript : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private int npcId;
    public string defaultGreeting;
    public string questGreeting;
    public int NPCId => npcId;

    [Header("Quest Settings")]
    [SerializeField] private List<QuestData> quests; // Quest list

    [Header("Icon References")]
    [SerializeField] private Image questStatusIcon; // Current quest icon above the NPC
    [SerializeField] private Sprite availableIcon;
    [SerializeField] private Sprite inProgressIcon;
    [SerializeField] private Sprite completedIcon;

    [Header("References")]
    [SerializeField] private ActionInteractable interactable;
    [SerializeField] private PlayerActionController actionController;

    void Awake()
    {
        interactable.SetAction(PerformInteraction);
    }

    void Start()
    {
        UpdateQuestIcon();
    }

    void OnEnable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStarted += HandleQuestChanged;
            QuestManager.Instance.OnQuestProgressCompleted += HandleQuestChanged;
            QuestManager.Instance.OnQuestTurnedIn += HandleQuestChanged;
        }
    }
    void OnDisable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestStarted -= HandleQuestChanged;
            QuestManager.Instance.OnQuestProgressCompleted -= HandleQuestChanged;
            QuestManager.Instance.OnQuestTurnedIn -= HandleQuestChanged;
        }
    }

    private void HandleQuestChanged(Quest quest)
    {
        if (!quests.Contains(quest.Data)) return;

        UpdateQuestIcon();
    }

    private void UpdateQuestIcon()
    {
        if (questStatusIcon == null || quests == null) return;

        QuestData displayQuest = null;
        Quest activeQuest = null;

        foreach (var q in quests)
        {
            var quest = QuestManager.Instance.GetActiveQuest(q.Id);
            if (quest != null && !quest.IsTurnedIn)
            {
                displayQuest = q;
                activeQuest = quest;
                break;
            }
            else if (!QuestManager.Instance.IsQuestCompleted(q.Id))
            {
                displayQuest = q;
                break;
            }
        }

        if (displayQuest == null)
        {
            questStatusIcon.gameObject.SetActive(false);
            return;
        }

        if (activeQuest == null) // Available
        {
            questStatusIcon.sprite = availableIcon;
        }
        else if (!activeQuest.IsCompleted) // In progress
        {
            questStatusIcon.sprite = inProgressIcon;
        }
        else if (activeQuest.IsCompleted && !activeQuest.IsTurnedIn) // Ready to turn in
        {
            questStatusIcon.sprite = completedIcon;
        }

        questStatusIcon.gameObject.SetActive(true);
    }

    private IEnumerator PerformInteraction()
    {
        NPCUIManager.Instance.OpenNPCDialogue(this);
        yield return null;
    }

    // Helpers
    public int QuestsAvailable() => quests?.Count ?? 0;
    public List<QuestData> GetQuests() => quests;
}
