using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for all minigame UI windows.
/// This class handles the generic UI flow:
/// - opening and closing the minigame window
/// - shared UI references
/// - feedback/instruction/title text
/// - completion/failure events
/// - future EXP reward integration
/// </summary>
public class MinigameBaseUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] protected GameObject rootPanel;        // Main root object of the minigame window
    [SerializeField] protected TMP_Text titleText;          // Title shown at the top of the minigame
    [SerializeField] protected TMP_Text instructionText;    // Main instruction text
    [SerializeField] protected TMP_Text feedbackText;       // Feedback text for success/failure/messages
    [SerializeField] protected Button closeButton;          // Button used to close the minigame
    [SerializeField] protected RectTransform gameArea;      // Optional area used by child minigames for content

    [Header("Reward")]
    [SerializeField] protected int expReward;               // EXP reward configured per minigame in Inspector
    public int GetExp => expReward;                         // Public read-only access to the reward value

    protected bool isCompleted;                             // True when the minigame has been completed
    protected bool isRunning;                               // True while the minigame is active/running

    // Events prepared for future systems (XP, analytics, progression, etc.)
    public event Action<MinigameBaseUI> OnMinigameCompleted;
    public event Action<MinigameBaseUI> OnMinigameFailed;

    /// <summary>
    /// Called by Unity when the object is initialized.
    /// Connects the close button to the shared CloseMinigame method.
    /// </summary>
    protected virtual void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMinigame);
    }

    /// <summary>
    /// Opens the minigame window and starts the shared open flow.
    /// Child minigames should not usually override this directly.
    /// Instead, they should override OnMinigameOpened().
    /// </summary>
    public virtual void OpenMinigame()
    {
        // Stop any previously running coroutines before reopening
        StopAllCoroutines();

        if (rootPanel != null)
            rootPanel.SetActive(true);

        isRunning = true;
        isCompleted = false;

        SetFeedback("");
        OnMinigameOpened();
    }

    /// <summary>
    /// Hook for child classes to run their specific startup logic
    /// after the generic open flow is finished.
    /// </summary>
    protected virtual void OnMinigameOpened()
    {
    }

    /// <summary>
    /// Closes the minigame window.
    /// </summary>
    public virtual void CloseMinigame()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);

        isRunning = false;
    }

    /// <summary>
    /// Sets the title text if assigned.
    /// </summary>
    public virtual void SetTitle(string newTitle)
    {
        if (titleText != null)
            titleText.text = newTitle;
    }

    /// <summary>
    /// Sets the instruction text if assigned.
    /// </summary>
    public virtual void SetInstruction(string newInstruction)
    {
        if (instructionText != null)
            instructionText.text = newInstruction;
    }

    /// <summary>
    /// Sets the feedback text if assigned.
    /// </summary>
    public virtual void SetFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;
    }

    /// <summary>
    /// Marks the minigame as completed and notifies listeners.
    /// Child minigames should call this when the player truly succeeds.
    /// </summary>
    protected virtual void CompleteMinigame()
    {
        isCompleted = true;
        isRunning = false;
        Debug.Log($"{gameObject.name} completed.");

        OnMinigameCompleted?.Invoke(this);

        // TODO: Hook minigame completion to the skill XP reward system.
    }

    /// <summary>
    /// Marks the minigame as failed and notifies listeners.
    /// Use this only for a true final failure state,
    /// not for temporary mistakes that simply restart a round.
    /// </summary>
    protected virtual void FailMinigame()
    {
        isRunning = false;
        Debug.Log($"{gameObject.name} failed.");

        OnMinigameFailed?.Invoke(this);
    }

    /// <summary>
    /// Returns the shared game area reference.
    /// Useful for child minigames that dynamically create UI elements.
    /// </summary>
    public RectTransform GetGameArea()
    {
        return gameArea;
    }
}