using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI controller for the balancing minigame.
/// Handles countdown, timer, setup of parameter rows, simulation result,
/// success/failure feedback, and restart flow.
/// Gameplay rules are handled by BalancingLogic.
/// </summary>
public class BalancingMinigameUI : MinigameBaseUI
{
    [Header("Balancing UI")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button simulateButton;

    [Header("Character Visuals")]
    [SerializeField] private Image playerImage;
    [SerializeField] private Image monsterImage;

    [Header("Parameter Rows")]
    [SerializeField] private BalanceParameterRowUI hpRow;
    [SerializeField] private BalanceParameterRowUI damageRow;
    [SerializeField] private BalanceParameterRowUI speedRow;
    [SerializeField] private BalanceParameterRowUI intelligenceRow;

    [Header("Parameter Icons")]
    [SerializeField] private Sprite hpIcon;
    [SerializeField] private Sprite damageIcon;
    [SerializeField] private Sprite speedIcon;
    [SerializeField] private Sprite intelligenceIcon;

    [Header("Level Config")]
    [SerializeField] private int level = 1;
    [SerializeField] private MonsterLevelConfig level1Config;
    [SerializeField] private MonsterLevelConfig level2Config;
    [SerializeField] private MonsterLevelConfig level3Config;

    [Header("Timing")]
    [SerializeField] private float totalRoundTime = 30f;
    [SerializeField] private float restartDelay = 1.5f;
    [SerializeField] private float successCloseDelay = 3f;

    private BalancingLogic logic;
    private MonsterLevelConfig currentConfig;
    private Coroutine timerRoutine;

    private MonsterStatValues currentValues;
    private bool canSimulate;

    protected override void Awake()
    {
        base.Awake();

        if (simulateButton != null)
            simulateButton.onClick.AddListener(OnSimulatePressed);
    }

    protected override void OnMinigameOpened()
    {
        logic = new BalancingLogic();
        currentConfig = GetCurrentConfig();

        if (currentConfig == null)
        {
            SetFeedback("Missing level configuration.");
            return;
        }

        StartCoroutine(RunBalancingGame());
    }

    private IEnumerator RunBalancingGame()
    {
        ResetVisualState();
        ApplyLevelConfig();

        SetInstruction("Adjust the values to set a fair challenge.");

        yield return StartCoroutine(ShowCountdown());

        canSimulate = true;

        if (simulateButton != null)
            simulateButton.interactable = true;

        timerRoutine = StartCoroutine(RunRoundTimer());
    }

    private void ResetVisualState()
    {
        StopExistingCoroutines();

        canSimulate = false;

        if (simulateButton != null)
            simulateButton.interactable = false;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
            countdownText.text = string.Empty;
        }

        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(totalRoundTime)}";

        if (resultText != null)
            resultText.text = string.Empty;

        SetFeedback(string.Empty);
    }

    private void ApplyLevelConfig()
    {
        if (currentConfig == null)
            return;

        SetTitle(currentConfig.levelTitle);

        if (playerImage != null && currentConfig.playerSprite != null)
            playerImage.sprite = currentConfig.playerSprite;

        if (monsterImage != null && currentConfig.monsterSprite != null)
            monsterImage.sprite = currentConfig.monsterSprite;

        currentValues = new MonsterStatValues(
            currentConfig.hpStartValue,
            currentConfig.damageStartValue,
            currentConfig.speedStartValue,
            currentConfig.intelligenceStartValue
        );

        hpRow.Setup("HP", hpIcon, currentConfig.hpStartValue, currentConfig.hpFixed, OnParameterChanged);
        damageRow.Setup("Damage", damageIcon, currentConfig.damageStartValue, currentConfig.damageFixed, OnParameterChanged);
        speedRow.Setup("Speed", speedIcon, currentConfig.speedStartValue, currentConfig.speedFixed, OnParameterChanged);
        intelligenceRow.Setup("Intelligence", intelligenceIcon, currentConfig.intelligenceStartValue, currentConfig.intelligenceFixed, OnParameterChanged);
    }

    private IEnumerator ShowCountdown()
    {
        if (countdownText == null)
            yield break;

        countdownText.gameObject.SetActive(true);

        countdownText.text = "Combat Balance";
        yield return new WaitForSeconds(1f);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
    }

    private IEnumerator RunRoundTimer()
    {
        float remaining = totalRoundTime;

        while (remaining > 0f)
        {
            if (timerText != null)
                timerText.text = $"Time: {Mathf.CeilToInt(remaining)}";

            remaining -= Time.deltaTime;
            yield return null;
        }

        if (timerText != null)
            timerText.text = "Time: 0";

        StartCoroutine(HandleFailure("Time is over! Restarting..."));
    }

    private void OnParameterChanged(MonsterParameterType parameterType, int newValue)
    {
        switch (parameterType)
        {
            case MonsterParameterType.HP:
                currentValues.hp = newValue;
                break;

            case MonsterParameterType.Damage:
                currentValues.damage = newValue;
                break;

            case MonsterParameterType.Speed:
                currentValues.speed = newValue;
                break;

            case MonsterParameterType.Intelligence:
                currentValues.intelligence = newValue;
                break;
        }
    }

    private void OnSimulatePressed()
    {
        if (!isRunning || !canSimulate || currentConfig == null)
            return;

        canSimulate = false;

        if (simulateButton != null)
            simulateButton.interactable = false;

        MonsterWeights weights = new MonsterWeights(
            currentConfig.hpWeight,
            currentConfig.damageWeight,
            currentConfig.speedWeight,
            currentConfig.intelligenceWeight
        );

        BalanceEvaluationResult result = logic.Evaluate(weights, currentValues);

        if (resultText != null)
        {
            resultText.text =
                $"Final Score: {result.finalScore:0.0}/100\n" +
                $"Damage to Monster: {result.normalizedDamage:0.0}/10";
        }

        switch (result.outcome)
        {
            case BalanceOutcome.TooHard:
                StartCoroutine(HandleFailure("Too hard! The player dies. Restarting..."));
                break;

            case BalanceOutcome.TooEasy:
                StartCoroutine(HandleFailure("Too easy! No challenge. Restarting..."));
                break;

            case BalanceOutcome.Balanced:
                StartCoroutine(HandleSuccess("Balanced! Challenging but fair."));
                break;
        }
    }

    private IEnumerator HandleFailure(string message)
    {
        if (!isRunning)
            yield break;

        isRunning = false;
        StopExistingCoroutines();

        SetFeedback(message);

        yield return new WaitForSeconds(restartDelay);

        if (rootPanel != null && rootPanel.activeInHierarchy)
            OpenMinigame();
    }

    private IEnumerator HandleSuccess(string message)
    {
        if (!isRunning)
            yield break;

        StopExistingCoroutines();
        CompleteMinigame();

        SetFeedback(message);

        yield return new WaitForSeconds(successCloseDelay);

        CloseMinigame();
    }

    private void StopExistingCoroutines()
    {
        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }
    }

    private MonsterLevelConfig GetCurrentConfig()
    {
        return level switch
        {
            1 => level1Config,
            2 => level2Config,
            3 => level3Config,
            _ => level1Config
        };
    }

    public override void CloseMinigame()
    {
        StopExistingCoroutines();
        base.CloseMinigame();
    }
}

/// <summary>
/// Inspector configuration for one monster/level.
/// </summary>
[System.Serializable]
public class MonsterLevelConfig
{
    [Header("Presentation")]
    public string levelTitle = "Combat Balance";
    public Sprite playerSprite;
    public Sprite monsterSprite;

    [Header("Weights (must sum to 10)")]
    public float hpWeight = 2.5f;
    public float damageWeight = 2.5f;
    public float speedWeight = 2.5f;
    public float intelligenceWeight = 2.5f;

    [Header("HP")]
    public bool hpFixed;
    [Range(1, 10)] public int hpStartValue = 5;

    [Header("Damage")]
    public bool damageFixed;
    [Range(1, 10)] public int damageStartValue = 5;

    [Header("Speed")]
    public bool speedFixed;
    [Range(1, 10)] public int speedStartValue = 5;

    [Header("Intelligence")]
    public bool intelligenceFixed;
    [Range(1, 10)] public int intelligenceStartValue = 5;
}