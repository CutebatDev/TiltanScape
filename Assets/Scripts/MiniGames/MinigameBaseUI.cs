using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinigameBaseUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] protected GameObject rootPanel;
    [SerializeField] protected TMP_Text titleText;
    [SerializeField] protected TMP_Text instructionText;
    [SerializeField] protected TMP_Text feedbackText;
    [SerializeField] protected Button closeButton;
    [SerializeField] protected RectTransform gameArea;

    protected bool isCompleted;
    protected bool isRunning;

    protected virtual void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMinigame);
    }

    public virtual void OpenMinigame()
    {
        if (rootPanel != null)
            rootPanel.SetActive(true);

        isRunning = true;
        isCompleted = false;
        SetFeedback("");
    }

    public virtual void CloseMinigame()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);

        isRunning = false;
    }

    public virtual void SetTitle(string newTitle)
    {
        if (titleText != null)
            titleText.text = newTitle;
    }

    public virtual void SetInstruction(string newInstruction)
    {
        if (instructionText != null)
            instructionText.text = newInstruction;
    }

    public virtual void SetFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;
    }

    protected virtual void CompleteMinigame()
    {
        isCompleted = true;
        isRunning = false;
        SetFeedback("Completed!");
        Debug.Log($"{gameObject.name} completed.");
    }

    protected virtual void FailMinigame()
    {
        isRunning = false;
        SetFeedback("Failed!");
        Debug.Log($"{gameObject.name} failed.");
    }

    public RectTransform GetGameArea()
    {
        return gameArea;
    }
}