using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ActionInteractable))]
[RequireComponent(typeof(PlayerActionController))]
public class QuestStation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QuestData questData;
    [SerializeField] private PlayerSkills playerSkills;

    [SerializeField] private ActionInteractable interactable;
    [SerializeField] private PlayerActionController actionController;
    

    void Awake()
    {
        interactable.SetAction(PerformQuestProgress);
    }

    private IEnumerator PerformQuestProgress()
    {
        Quest quest = QuestManager.Instance.StartQuest(questData);

        float averageMultiplier = 1f;
        if (questData.relevantSkills != null && questData.relevantSkills.Count > 0)
        {
            float sum = 0f;
            foreach (var skill in questData.relevantSkills)
            {
                int level = playerSkills.GetLevel(skill);
                sum += skill.actionSpeed.Evaluate(level);
            }
            averageMultiplier = sum / questData.relevantSkills.Count;
        }

        float timer = 0f;


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
