using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject questButtonPrefab;

    private QuestStation currentStation;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }    

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowQuestSelection(QuestStation station, List<QuestData> availableQuests)
    {
        if (availableQuests == null || availableQuests.Count == 0)
        {
            Debug.Log("Empty");
            return;
        }

        currentStation = station;

        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        foreach (var quest in availableQuests)
        {
            GameObject buttonObj = Instantiate(questButtonPrefab, buttonParent);
            Button button = buttonObj.GetComponent<Button>();
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();

            string skillList = "";
            if (quest.relevantSkills != null && quest.relevantSkills.Count > 0)
            {
                for (int i = 0; i < quest.relevantSkills.Count; i++)
                {
                    skillList += quest.relevantSkills[i].skillName;
                    if (i < quest.relevantSkills.Count - 1)
                        skillList += ", ";
                }
            }

            buttonText.text = $"{quest.Title}\nSkills: {skillList}";

            QuestData capturedQuest = quest;
            button.onClick.AddListener(() => OnQuestChosen(capturedQuest));
        }

        panel.SetActive(true);
    }

    public void OnQuestChosen(QuestData quest)
    {
        if (currentStation == null || quest == null) return;

        currentStation.OnQuestSelected(quest);

        panel.SetActive(false);
    }

    public void CloseUI()
    {
        panel.SetActive(false);
    }
}
