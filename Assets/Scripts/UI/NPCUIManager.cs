using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCUIManager : MonoBehaviour
{
    public static NPCUIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform questButtonContainer;
    [SerializeField] private TMP_Text npcMessageText;

    [Header("References Inside Quest")]
    [SerializeField] private TMP_Text questDetailTitle;
    [SerializeField] private TMP_Text questDetailDescription;
    [SerializeField] private TMP_Text questDetailSkills;
    [SerializeField] private Button acceptQuestButton;

    [Header("Quest Button")]
    [SerializeField] private GameObject questButtonPrefab;
    [SerializeField] private TMP_Text questButtonLabel;

    [Header("Quest Button Colors")]
    [SerializeField] private Color availableQuestColor;
    [SerializeField] private Color inProgressQuestColor;
    [SerializeField] private Color completeQuestColor;

    [Header("Text Strings")]
    [SerializeField] private string skillReqText;
    [SerializeField] private string acceptButtonText;
    [SerializeField] private string turnInButtonText;

    private NPCScript currentNPC;
    private QuestData selectedQuest;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }    

        Instance = this;
        DontDestroyOnLoad(gameObject);

        acceptQuestButton.gameObject.SetActive(false);
        acceptQuestButton.onClick.AddListener(OnAcceptQuestClicked);
    }

    private void EnableQuestInfoUI()
    {
        npcMessageText.gameObject.SetActive(false);
        questDetailTitle.gameObject.SetActive(true);
        questDetailDescription.gameObject.SetActive(true);
        questDetailSkills.gameObject.SetActive(true);
        questButtonContainer.gameObject.SetActive(false);
    }

    private void EnableQuestListUI()
    {
        npcMessageText.gameObject.SetActive(true);
        questDetailTitle.gameObject.SetActive(false);
        questDetailDescription.gameObject.SetActive(false);
        questDetailSkills.gameObject.SetActive(false);
        questButtonContainer.gameObject.SetActive(true);
        panel.SetActive(true);
    }

    public void OpenNPCDialogue(NPCScript npc)
    {
        currentNPC = npc;
        selectedQuest = null;

        foreach (Transform child in questButtonContainer)
            Destroy(child.gameObject);

        if (npc.QuestsAvailable() == 0)
        {
            npcMessageText.text = npc.defaultGreeting;
            acceptQuestButton.gameObject.SetActive(false);
            EnableQuestListUI();
            return;
        }

        npcMessageText.text = npc.questGreeting;

        foreach (var q in npc.GetQuests())
        {
            GameObject btnObj = Instantiate(questButtonPrefab, questButtonContainer);
            TMP_Text btnText = btnObj.transform.GetChild(0).GetComponent<TMP_Text>();
            Button btn = btnObj.GetComponent<Button>();

            var activeQuest = QuestManager.Instance.GetActiveQuest(q.Id);

            if (activeQuest == null)
            {
                btnText.color = availableQuestColor;
                btnText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(availableQuestColor)}>[AVAILABLE] {q.Title}</color>";
                Debug.Log($"{q.Title}'s Color = available");
            }
            else if (!activeQuest.IsCompleted)
            {
                btnText.color = inProgressQuestColor;
                btnText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(inProgressQuestColor)}>[IN-PROGRESS] {q.Title}</color>";
                Debug.Log($"{q.Title}'s Color = in progress");
            }
            else if (activeQuest.IsCompleted && !activeQuest.IsTurnedIn)
            {
                btnText.color = completeQuestColor;
                btnText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(completeQuestColor)}>[COMPLETED] {q.Title}</color>";
                Debug.Log($"{q.Title}'s Color = completed");
            }

            //btnText.text = q.Title;

            QuestData capturedQuest = q;
            btn.onClick.AddListener(() => OnQuestSelected(capturedQuest));
        }

        EnableQuestListUI();
    }

    private void OnQuestSelected(QuestData quest)
    {
        selectedQuest = quest;
        ShowQuestDetails(quest);

        var activeQuest = QuestManager.Instance.GetActiveQuest(quest.Id);
        if (activeQuest == null)
        {
            acceptQuestButton.gameObject.SetActive(true);
            acceptQuestButton.GetComponentInChildren<TMP_Text>().text = acceptButtonText;
        }
        else if (activeQuest.IsCompleted && !activeQuest.IsTurnedIn)
        {
            acceptQuestButton.gameObject.SetActive(true);
            acceptQuestButton.GetComponentInChildren<TMP_Text>().text = turnInButtonText;
        }
        else
        {
            acceptQuestButton.gameObject.SetActive(false);
        }

        EnableQuestInfoUI();
    }

    private void OnAcceptQuestClicked()
    {
        if (selectedQuest == null) return;

        var activeQuest = QuestManager.Instance.GetActiveQuest(selectedQuest.Id);

        if (activeQuest == null)
        {
            QuestManager.Instance.StartQuest(selectedQuest);
        }
        else if (activeQuest.IsCompleted && !activeQuest.IsTurnedIn)
        {
            QuestManager.Instance.TurnInQuest(selectedQuest.Id);
        }

        OnQuestSelected(selectedQuest);
        CloseDialogue();
    }

    public void ShowQuestDetails(QuestData quest)
    {
        questDetailTitle.text = quest.Title;
        questDetailDescription.text = quest.Description;

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

        questDetailSkills.text = $"{skillReqText}{skillList}";
    }

    public void CloseDialogue()
    {
        panel.SetActive (false);
        selectedQuest = null;
        currentNPC = null;
        acceptQuestButton.gameObject.SetActive(false);
    }
}
