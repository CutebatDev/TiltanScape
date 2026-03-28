using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(ActionInteractable))]
public class QuestStation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<QuestData> availableQuests;
    [SerializeField] private PlayerSkills playerSkills;

    [SerializeField] private ActionInteractable interactable;
    [SerializeField] private PlayerActionController actionController;

    private QuestData selectedQuest;

    void Awake()
    {
        interactable.SetAction(OpenQuestSelectionCoroutine);
    }

    private IEnumerator OpenQuestSelectionCoroutine()
    {
        QuestUIManager.Instance.ShowQuestSelection(this, availableQuests);
        yield return null;
    }

    public void OnQuestSelected(QuestData quest)
    {
        SelectQuest(quest);
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
        Quest quest = QuestManager.Instance.StartQuest(selectedQuest);

        float averageMultiplier = 1f;
        if (selectedQuest.relevantSkills != null && selectedQuest.relevantSkills.Count > 0)
        {
            float sum = 0f;
            foreach (var skill in selectedQuest.relevantSkills)
            {
                int level = playerSkills.GetLevel(skill);
                sum += skill.actionSpeed.Evaluate(level);
            }
            averageMultiplier = sum / selectedQuest.relevantSkills.Count;
        }

        float timer = 0f;


        while (timer < selectedQuest.baseActionTime)
        {
            if (actionController.ShouldCancelAction())
            {
                selectedQuest = null;
                yield break;
            }

            // Avg multiplier to actual quest progress
            float deltaProgress = (Time.deltaTime * averageMultiplier) / selectedQuest.baseActionTime;
            QuestManager.Instance.AddProgress(selectedQuest.Id, deltaProgress * selectedQuest.baseActionTime);

            // For timer UI
            timer += Time.deltaTime * averageMultiplier;

            yield return null;
        }

        Debug.Log($"Quest {selectedQuest.Title} progress completed!");
        selectedQuest = null;
    }
}
