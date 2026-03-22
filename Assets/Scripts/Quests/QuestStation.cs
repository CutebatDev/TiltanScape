using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class QuestStation : Interactable
{
    [SerializeField] private QuestData questData;
    [SerializeField] private PlayerActionController actionController;
    [SerializeField] private PlayerSkills playerSkills;

    void Awake()
    {
        OnInteract.AddListener(StartInteract);
    }

    public void StartInteract()
    {
        if (actionController.IsBusy) return;

        // Ensure quest exists
        QuestManager.Instance.StartQuest(questData);

        // Start the action coroutine
        actionController.StartAction(PerformQuestProgress());
    }

    private IEnumerator PerformQuestProgress()
    {
        Quest quest = QuestManager.Instance.GetActiveQuest(questData.Id);
        if (quest == null)
            yield break;

        float timer = 0f;

        float averageMultiplier = 1f;
        if (questData.relevantSkills != null && questData.relevantSkills.Count > 0)
        {
            float sum = 0f;
            foreach (var skill in questData.relevantSkills)
            {
                int level = playerSkills.GetLevel(skill);
                float multiplier = skill.actionSpeed.Evaluate(level);
                sum += multiplier;
            }
            averageMultiplier = sum / questData.relevantSkills.Count;
        }

        while (timer < questData.baseActionTime)
        {
            if (actionController.ShouldCancelAction())
                yield break;

            // Avg multiplier to actual quest progress
            float deltaProgress = (Time.deltaTime * averageMultiplier) / questData.baseActionTime;
            QuestManager.Instance.AddProgress(questData.Id, deltaProgress * questData.baseActionTime);

            // For timer UI
            timer += Time.deltaTime * averageMultiplier;

            yield return null;
        }

        Debug.Log($"Quest {questData.Title} progress completed!");
    }
}
