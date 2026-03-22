using System.Collections;
using UnityEngine;

public class NPCScript : Interactable
{
    [Header("Quest Settings")]
    [SerializeField] private QuestData questData; // Quest list
    [SerializeField] private GameObject questIcon; // Current quest icon above the NPC

    [Header("References")]
    [SerializeField] private PlayerActionController actionController;
    [SerializeField] private float baseInteractTime = 1f;

    void Awake()
    {
        OnInteract.AddListener(StartInteract);    
    }

    void Update()
    {
        UpdateQuestIcon();
    }

    private void UpdateQuestIcon()
    {
        if (questIcon == null || questData == null)
            return;

        Quest activeQuest = QuestManager.Instance.GetActiveQuest(questData.Id);

        questIcon.SetActive(activeQuest == null && !QuestManager.Instance.IsQuestCompleted(questData.Id));
    }

    public void StartInteract()
    {
        if (actionController.IsBusy) return;

        actionController.StartAction(PerformInteraction());
    }

    private IEnumerator PerformInteraction()
    {
        float timer = 0f;

        while (timer < baseInteractTime)
        {
            if (actionController.ShouldCancelAction())
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

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
    }
}
