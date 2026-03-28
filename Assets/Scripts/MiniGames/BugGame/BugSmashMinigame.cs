using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI controller for the bug smash minigame.
/// Handles countdown, timed spawning, timer, restart flow,
/// success/failure feedback, and bug interactions.
/// Gameplay rules are handled by BugSmashLogic.
/// </summary>
public class BugSmashMinigame : MinigameBaseUI
{
    [Header("Bug Smash UI")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text counterText;

    [Header("Bug Prefab")]
    [SerializeField] private GameObject bugPrefab;

    [Header("Bug Sprites")]
    [SerializeField] private Sprite aliveBugSprite;
    [SerializeField] private Sprite smashedBugSprite;

    [Header("Level Settings")]
    [SerializeField] private int level = 1;

    [Header("Gameplay Timing")]
    [SerializeField] private float spawnDuration = 15f;       // Total period during which bugs appear one by one
    [SerializeField] private float totalRoundTime = 30f;      // Maximum time to finish the level
    [SerializeField] private float restartDelay = 1.5f;       // Delay after failure before restarting
    [SerializeField] private float successCloseDelay = 3f;    // Delay before closing after success

    [Header("Spawn Padding")]
    [SerializeField] private float outsideSpawnOffset = 80f;  // How far outside the area corners the bug starts

    private BugSmashLogic logic;
    private readonly List<BugActorUI> activeBugs = new();

    private Coroutine roundTimerRoutine;
    private Coroutine spawnRoutine;

    public bool IsGameplayRunning => isRunning && logic != null && !logic.IsCompleted;

    protected override void OnMinigameOpened()
    {
        logic = new BugSmashLogic();
        logic.ConfigureLevel(level);

        StartCoroutine(RunBugSmashGame());
    }

    /// <summary>
    /// Main round flow.
    /// </summary>
    private IEnumerator RunBugSmashGame()
    {
        ResetVisualState();
        SetInstruction("Eliminate the bugs");

        yield return StartCoroutine(ShowCountdown());

        logic.BeginInput();

        spawnRoutine = StartCoroutine(SpawnBugsOverTime());
        roundTimerRoutine = StartCoroutine(RunRoundTimer());
    }

    /// <summary>
    /// Resets all round visuals and state.
    /// </summary>
    private void ResetVisualState()
    {
        StopExistingGameplayCoroutines();

        logic.ResetRound();
        ClearAllBugs();

        countdownText.gameObject.SetActive(false);

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        if (counterText != null)
            counterText.gameObject.SetActive(true);

        UpdateTimerText(totalRoundTime);
        UpdateCounterText();
        SetFeedback("");
    }

    /// <summary>
    /// Shows the initial countdown.
    /// </summary>
    private IEnumerator ShowCountdown()
    {
        countdownText.gameObject.SetActive(true);

        countdownText.text = "Eliminate the bugs";
        yield return new WaitForSeconds(1f);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Spawns bugs progressively over the configured spawn duration.
    /// </summary>
    private IEnumerator SpawnBugsOverTime()
    {
        int total = logic.TotalBugCount;

        if (total <= 0)
            yield break;

        float interval = spawnDuration / total;

        for (int i = 0; i < total; i++)
        {
            SpawnSingleBug();
            logic.RegisterSpawn();
            UpdateCounterText();

            if (i < total - 1)
                yield return new WaitForSeconds(interval);
        }
    }

    /// <summary>
    /// Controls the 30-second round timer.
    /// </summary>
    private IEnumerator RunRoundTimer()
    {
        float remaining = totalRoundTime;

        while (remaining > 0f)
        {
            UpdateTimerText(remaining);

            if (logic.IsCompleted)
                yield break;

            remaining -= Time.deltaTime;
            yield return null;
        }

        UpdateTimerText(0f);

        if (!logic.IsCompleted)
        {
            StartCoroutine(HandleFailure());
        }
    }

    /// <summary>
    /// Called by a bug when the player tries to smash it.
    /// </summary>
    public void TrySmashBug(BugActorUI bug)
    {
        if (!isRunning || logic == null || !logic.AcceptingInput)
            return;

        if (bug == null || bug.IsSmashed)
            return;

        bool counted = logic.RegisterSmash();

        if (!counted)
            return;

        bug.Smash();
        UpdateCounterText();

        if (logic.IsCompleted)
        {
            StartCoroutine(HandleSuccess());
        }
    }

    private void SpawnSingleBug()
    {
        if (bugPrefab == null || gameArea == null)
            return;

        GameObject bugObject = Instantiate(bugPrefab, gameArea);
        BugActorUI bugActor = bugObject.GetComponent<BugActorUI>();

        if (bugActor == null)
        {
            Debug.LogWarning("Bug prefab is missing BugActorUI component.");
            Destroy(bugObject);
            return;
        }

        RectTransform bugRect = bugObject.GetComponent<RectTransform>();

        Vector2 spawnPosition = GetRandomCornerOutsidePosition(bugRect);
        Vector2 firstTarget = GetRandomPointNearInsideFromCorner();

        bugActor.Setup(
            this,
            gameArea,
            aliveBugSprite,
            smashedBugSprite,
            logic.MoveSpeed,
            spawnPosition,
            firstTarget
        );

        activeBugs.Add(bugActor);
    }

    private Vector2 GetRandomCornerOutsidePosition(RectTransform bugRect)
    {
        Rect rect = gameArea.rect;

        float halfWidth = bugRect.rect.width * 0.5f;
        float halfHeight = bugRect.rect.height * 0.5f;

        int corner = Random.Range(0, 4);

        switch (corner)
        {
            case 0: // Top-left
                return new Vector2(rect.xMin - halfWidth - outsideSpawnOffset, rect.yMax + halfHeight + outsideSpawnOffset);

            case 1: // Top-right
                return new Vector2(rect.xMax + halfWidth + outsideSpawnOffset, rect.yMax + halfHeight + outsideSpawnOffset);

            case 2: // Bottom-left
                return new Vector2(rect.xMin - halfWidth - outsideSpawnOffset, rect.yMin - halfHeight - outsideSpawnOffset);

            default: // Bottom-right
                return new Vector2(rect.xMax + halfWidth + outsideSpawnOffset, rect.yMin - halfHeight - outsideSpawnOffset);
        }
    }

    private Vector2 GetRandomPointNearInsideFromCorner()
    {
        Rect rect = gameArea.rect;

        float marginX = rect.width * 0.2f;
        float marginY = rect.height * 0.2f;

        float x = Random.Range(rect.xMin + marginX, rect.xMax - marginX);
        float y = Random.Range(rect.yMin + marginY, rect.yMax - marginY);

        return new Vector2(x, y);
    }

    private void UpdateTimerText(float remainingTime)
    {
        if (timerText == null)
            return;

        int seconds = Mathf.CeilToInt(remainingTime);
        timerText.text = $"Time: {seconds}";
    }

    private void UpdateCounterText()
    {
        if (counterText == null || logic == null)
            return;

        counterText.text = $"Bugs: {logic.SmashedBugCount}/{logic.TotalBugCount}";
    }

    private IEnumerator HandleFailure()
    {
        if (!isRunning)
            yield break;

        logic.StopInput();
        isRunning = false;

        StopExistingGameplayCoroutines();

        SetFeedback("You failed! Restarting...");

        yield return new WaitForSeconds(restartDelay);

        if (rootPanel != null && rootPanel.activeInHierarchy)
        {
            OpenMinigame();
        }
    }

    private IEnumerator HandleSuccess()
    {
        if (!isRunning)
            yield break;

        logic.StopInput();
        StopExistingGameplayCoroutines();

        CompleteMinigame();

        string levelText = GetLevelOrdinalText();
        SetFeedback($"You passed your {levelText} extermination test!");

        yield return new WaitForSeconds(successCloseDelay);

        CloseMinigame();
    }

    private void StopExistingGameplayCoroutines()
    {
        if (roundTimerRoutine != null)
        {
            StopCoroutine(roundTimerRoutine);
            roundTimerRoutine = null;
        }

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private void ClearAllBugs()
    {
        for (int i = activeBugs.Count - 1; i >= 0; i--)
        {
            if (activeBugs[i] != null)
                Destroy(activeBugs[i].gameObject);
        }

        activeBugs.Clear();

        if (gameArea == null)
            return;

        for (int i = gameArea.childCount - 1; i >= 0; i--)
        {
            Destroy(gameArea.GetChild(i).gameObject);
        }
    }

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

    public override void CloseMinigame()
    {
        StopExistingGameplayCoroutines();
        ClearAllBugs();
        base.CloseMinigame();
    }
}