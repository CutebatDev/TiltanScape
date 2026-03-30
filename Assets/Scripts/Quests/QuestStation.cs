using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(ActionInteractable))]
public class QuestStation : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private ActionInteractable interactable;

    private List<QuestData> availableQuests;

    private QuestData selectedQuest;

    void Awake()
    {
        if (!interactable)
            interactable = GetComponent<ActionInteractable>();

        interactable.SetAction(OpenQuestSelectionCoroutine);
    }

    private IEnumerator OpenQuestSelectionCoroutine()
    {
        yield return new WaitUntil(() => QuestUIManager.Instance != null);

        List<Quest> activeQuests = QuestManager.Instance.GetActiveQuests();

        if (activeQuests.Count == 0)
        {
            Debug.Log("QuestStation: No active quests to show");
            yield break;
        }

        availableQuests = activeQuests.Select(q => q.Data).ToList();

        QuestUIManager.Instance.ShowQuestSelection(this, availableQuests);
    }

    public void OnQuestSelected(QuestData quest)
    {
        SelectQuest(quest);
        //QuestUIManager.Instance.CloseUI();
        StartCoroutine(PerformQuestProgress());
    }

    public void SelectQuest(QuestData quest)
    {
        selectedQuest = null;

        if (!availableQuests.Contains(quest))
        {
            Debug.LogWarning($"Quest {quest.Title} is not available at this station.");
            return;
        }

        selectedQuest = quest;
        Debug.Log($"Selected quest: {selectedQuest.Title}");
    }

    private IEnumerator PerformQuestProgress()
    {
        if (selectedQuest == null)
        {
            Debug.LogWarning("No quest selected");
            yield break;
        }

        QuestData questToPerform = selectedQuest;

        Quest quest = QuestManager.Instance.GetActiveQuest(questToPerform.Id);
        if (quest == null)
        {
            Debug.LogWarning($"Quest {questToPerform.Title} is not active!");
            yield break;
        }

        float averageMultiplier = 1f;
        if (questToPerform.relevantSkills != null && questToPerform.relevantSkills.Count > 0)
        {
            float sum = 0f;
            foreach (var skill in questToPerform.relevantSkills)
            {
                int level = PlayerSkills.Instance.GetLevel(skill);
                sum += skill.actionSpeed.Evaluate(level);
            }

            averageMultiplier = sum / questToPerform.relevantSkills.Count;
        }

        float tickInterval = questToPerform.baseTickInterval / averageMultiplier;
        float progressPerTick = questToPerform.progressTickPercent;

        while (!quest.IsCompleted)
        {
            if (PlayerActionController.Instance.ShouldCancelAction())
            {
                yield break;
            }

            yield return new WaitForSeconds(tickInterval);

            QuestManager.Instance.AddProgress(questToPerform.Id, progressPerTick);

            Debug.Log($"Tick: +{questToPerform.progressTickPercent * 100f}% progress");
        }

        Debug.Log($"Quest {questToPerform.Title} progress completed!");
        selectedQuest = null;
    }
}