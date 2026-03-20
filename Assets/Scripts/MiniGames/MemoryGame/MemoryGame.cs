using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI controller for the memory minigame.
/// Handles visuals, countdown, sequence display, buttons, and feedback.
/// Gameplay rules are handled by MemoryGameLogic.
/// </summary>
public class MemoryGameMinigame : MinigameBaseUI
{
    [Header("Memory Game UI")]
    [SerializeField] private TMP_Text countdownText;           // Text used for the 3-2-1 countdown
    [SerializeField] private Image sequenceDisplay;            // Image that shows the sequence sprites
    [SerializeField] private GameObject choiceGrid;            // Parent object for the choice buttons

    [Header("Choice Button")]
    [SerializeField] private GameObject choiceButtonPrefab;    // Prefab used to build each clickable choice

    [Header("Memory Images")]
    [SerializeField] private List<Sprite> availableSprites = new(); // Sprites available for the sequence and choices

    [Header("Level Settings")]
    [SerializeField] private int level = 1;                    // Current difficulty level

    private const float totalSequenceTime = 6f;                // Total time reserved for showing the full sequence
    [SerializeField] private float gapBetweenImages = 0.2f;    // Short empty gap between one image and the next

    private MemoryGameLogic logic;                             // Pure gameplay logic for this minigame

    /// <summary>
    /// Called automatically by the base class when the minigame is opened.
    /// Creates the logic object, configures the level, and starts the game flow.
    /// </summary>
    protected override void OnMinigameOpened()
    {
        logic = new MemoryGameLogic();
        logic.ConfigureLevel(level);

        StartCoroutine(RunMemoryGame());
    }

    /// <summary>
    /// Main flow of the memory minigame.
    /// Resets the UI, shows countdown, generates the sequence,
    /// displays it, then creates the choice buttons.
    /// </summary>
    private IEnumerator RunMemoryGame()
    {
        ResetVisualState();
        SetInstruction("Memorize the sequence and replicate");

        yield return StartCoroutine(ShowCountdown());

        logic.GenerateSequence();
        yield return StartCoroutine(ShowSequence());

        BuildChoiceGrid();
        ShowChoices();
    }

    /// <summary>
    /// Resets temporary round state and hides all gameplay UI before starting again.
    /// </summary>
    private void ResetVisualState()
    {
        logic.ResetRound();

        countdownText.gameObject.SetActive(false);
        sequenceDisplay.gameObject.SetActive(false);
        choiceGrid.SetActive(false);

        sequenceDisplay.sprite = null;
        sequenceDisplay.enabled = true; // Make sure the image component is visible again for the next round
        SetFeedback("");

        ClearChoiceGrid();
    }

    /// <summary>
    /// Shows a simple countdown before the sequence starts.
    /// </summary>
    private IEnumerator ShowCountdown()
    {
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Displays the generated sequence one image at a time.
    /// A short blank gap is inserted between images so repeated sprites
    /// are easier for the player to notice.
    /// </summary>
    private IEnumerator ShowSequence()
    {
        sequenceDisplay.gameObject.SetActive(true);
        sequenceDisplay.enabled = true;

        // Reserve part of the total time for the gaps between images
        float totalGapTime = gapBetweenImages * (logic.ExposureCount - 1);
        float singleExposureTime = (totalSequenceTime - totalGapTime) / logic.ExposureCount;

        for (int i = 0; i < logic.GeneratedSequence.Count; i++)
        {
            int spriteIndex = logic.GeneratedSequence[i];

            if (spriteIndex >= 0 && spriteIndex < availableSprites.Count)
            {
                sequenceDisplay.sprite = availableSprites[spriteIndex];
            }

            // Show the current image
            sequenceDisplay.enabled = true;
            yield return new WaitForSeconds(singleExposureTime);

            // Hide briefly before the next image to create a visual break
            sequenceDisplay.enabled = false;

            if (i < logic.GeneratedSequence.Count - 1)
            {
                yield return new WaitForSeconds(gapBetweenImages);
            }
        }

        sequenceDisplay.gameObject.SetActive(false);
        sequenceDisplay.enabled = true; // Restore state for future rounds
    }

    /// <summary>
    /// Creates the clickable choice buttons based on the current level settings.
    /// </summary>
    private void BuildChoiceGrid()
    {
        ClearChoiceGrid();

        for (int i = 0; i < logic.ColorCount; i++)
        {
            GameObject buttonObject = Instantiate(choiceButtonPrefab, choiceGrid.transform);
            MemoryChoiceButton choiceButton = buttonObject.GetComponent<MemoryChoiceButton>();

            if (choiceButton != null && i < availableSprites.Count)
            {
                choiceButton.Setup(i, availableSprites[i], this);
            }
        }
    }

    /// <summary>
    /// Removes all current buttons from the choice grid.
    /// </summary>
    private void ClearChoiceGrid()
    {
        for (int i = choiceGrid.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(choiceGrid.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Shows the player's selectable buttons and allows input.
    /// </summary>
    private void ShowChoices()
    {
        choiceGrid.SetActive(true);
        logic.BeginInput();
        SetFeedback("Repeat the sequence.");
    }

    /// <summary>
    /// Receives the player's selected choice from MemoryChoiceButton
    /// and reacts based on the result returned by the gameplay logic.
    /// </summary>
    public void RegisterPlayerChoice(int selectedIndex)
    {
        MemoryChoiceResult result = logic.RegisterChoice(selectedIndex);

        switch (result)
        {
            case MemoryChoiceResult.Ignored:
                return;

            case MemoryChoiceResult.CorrectSoFar:
                return;

            case MemoryChoiceResult.Wrong:
                StartCoroutine(HandleFailure());
                return;

            case MemoryChoiceResult.Completed:
                StartCoroutine(HandleSuccess());
                return;
        }
    }

    /// <summary>
    /// Handles an incorrect sequence attempt.
    /// This is not a final minigame failure — the round simply restarts.
    /// </summary>
    private IEnumerator HandleFailure()
    {
        SetFeedback("Wrong sequence! Restarting...");

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(RunMemoryGame());
    }

    /// <summary>
    /// Handles successful completion of the minigame.
    /// Marks the minigame as completed, shows feedback, and closes it.
    /// </summary>
    private IEnumerator HandleSuccess()
    {
        CompleteMinigame();

        string levelText = GetLevelOrdinalText();
        SetFeedback($"You passed your {levelText} art test!");

        yield return new WaitForSeconds(3f);

        CloseMinigame();
    }

    /// <summary>
    /// Converts the numeric level into readable ordinal text.
    /// </summary>
    private string GetLevelOrdinalText()
    {
        switch (level)
        {
            case 1: return "first";
            case 2: return "second";
            case 3: return "third";
            default: return "first";
        }
    }
}