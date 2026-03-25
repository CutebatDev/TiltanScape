using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

[RequireComponent(typeof(ActionInteractable))]
[RequireComponent(typeof(PlayerActionController))]
public class NPCScript : MonoBehaviour
{
    [Header("Quest Settings")]
    [SerializeField] private QuestData questData; // Quest list

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
        if (quest.Data != questData) return;

        UpdateQuestIcon();
    }

    private void UpdateQuestIcon()
    {
        if (questStatusIcon == null || questData == null) return;

        Quest activeQuest = QuestManager.Instance.GetActiveQuest(questData.Id);

        if (activeQuest == null && !QuestManager.Instance.IsQuestCompleted(questData.Id))
        {
            // Quest available
            questStatusIcon.gameObject.SetActive(true);
            questStatusIcon.sprite = availableIcon;
        }
        else if (activeQuest != null && !activeQuest.IsCompleted)
        {
            // Quest in progress
            questStatusIcon.gameObject.SetActive(true);
            questStatusIcon.sprite = inProgressIcon;
        }
        else if (activeQuest != null && activeQuest.IsCompleted && !activeQuest.IsTurnedIn)
        {
            // Quest progress finished
            questStatusIcon.gameObject.SetActive(true);
            questStatusIcon.sprite = completedIcon;
        }
        else
        {
            // No quest
            questStatusIcon.gameObject.SetActive(false);
        }
    }

    private IEnumerator PerformInteraction()
    {
        Quest activeQuest = QuestManager.Instance.GetActiveQuest(questData.Id);

        if (activeQuest == null)
        {
            QuestManager.Instance.StartQuest(questData);
            Debug.Log($"Quest '{questData.Title}' started!");
        }
        else if (activeQuest.IsCompleted && !activeQuest.IsTurnedIn)
        {
            QuestManager.Instance.TurnInQuest(questData.Id);
            Debug.Log($"Quest '{questData.Title}' turned in!");
        }
        else
        {
            Debug.Log($"Quest '{questData.Title}' is in progress: {activeQuest.Progress * 100:F0}% complete.");
        }

        yield return null;
    }
}
