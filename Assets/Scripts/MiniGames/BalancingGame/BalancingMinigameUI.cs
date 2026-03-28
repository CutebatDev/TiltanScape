using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI controller for the balancing minigame.
/// Handles setup of parameter rows, battle sequence,
/// success/failure feedback, and restart flow.
/// Gameplay rules are handled by BalancingLogic.
/// </summary>
public class BalancingMinigameUI : MinigameBaseUI
{
    [Header("Balancing UI")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Button simulateButton;

    [Header("Character Visuals")]
    [SerializeField] private Image playerImage;
    [SerializeField] private Image monsterImage;

    [Header("Parameter Rows")]
    [SerializeField] private BalanceParameterRowUI hpRow;
    [SerializeField] private BalanceParameterRowUI damageRow;
    [SerializeField] private BalanceParameterRowUI speedRow;
    [SerializeField] private BalanceParameterRowUI intelligenceRow;

    [Header("Level Config")]
    [SerializeField] private int level = 1;
    [SerializeField] private MonsterLevelConfig level1Config;
    [SerializeField] private MonsterLevelConfig level2Config;
    [SerializeField] private MonsterLevelConfig level3Config;

    [Header("Flow Timing")]
    [SerializeField] private float restartDelay = 8f;
    [SerializeField] private float successCloseDelay = 8f;

    [Header("Battle Countdown")]
    [SerializeField] private float battleCountdownStepDelay = 2f;
    [SerializeField] private float battleCountdownFinalDelay = 2f;

    [Header("Battle Animation")]
    [SerializeField] private float moveDistance = 80f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float hitPause = 0.25f;
    [SerializeField] private float betweenActionsDelay = 1f;
    [SerializeField] private float returnDuration = 0.5f;

    private BalancingLogic logic;
    private MonsterLevelConfig currentConfig;

    private MonsterStatValues currentValues;
    private bool canSimulate;

    private RectTransform playerRect;
    private RectTransform monsterRect;
    private Vector2 playerStartPos;
    private Vector2 monsterStartPos;

    protected override void Awake()
    {
        base.Awake();

        if (simulateButton != null)
            simulateButton.onClick.AddListener(OnSimulatePressed);
    }

    private void OnDestroy()
    {
        if (simulateButton != null)
            simulateButton.onClick.RemoveListener(OnSimulatePressed);
    }

    protected override void OnMinigameOpened()
    {
        logic = new BalancingLogic();
        currentConfig = GetCurrentConfig();

        if (!HasRequiredReferences())
        {
            SetFeedback("Missing UI references.");
            return;
        }

        if (currentConfig == null)
        {
            SetFeedback("Missing level configuration.");
            return;
        }

        playerRect = playerImage != null ? playerImage.rectTransform : null;
        monsterRect = monsterImage != null ? monsterImage.rectTransform : null;

        StartCoroutine(RunBalancingGame());
    }

    private bool HasRequiredReferences()
    {
        return
            countdownText != null &&
            simulateButton != null &&
            playerImage != null &&
            monsterImage != null &&
            hpRow != null &&
            damageRow != null &&
            speedRow != null &&
            intelligenceRow != null;
    }

    private IEnumerator RunBalancingGame()
    {
        ResetVisualState();
        ApplyLevelConfig();

        SetInstruction("Adjust the values to set a fair challenge.");

        yield return StartCoroutine(ShowSetupCountdown());

        canSimulate = true;

        if (simulateButton != null)
            simulateButton.interactable = true;
    }

    private void ResetVisualState()
    {
        canSimulate = false;

        if (simulateButton != null)
            simulateButton.interactable = false;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
            countdownText.text = string.Empty;
        }

        SetFeedback(string.Empty);
    }

    private void ApplyLevelConfig()
    {
        if (currentConfig == null)
            return;

        SetTitle(currentConfig.levelTitle);

        if (playerImage != null && currentConfig.playerSprites != null && currentConfig.playerSprites.idle != null)
            playerImage.sprite = currentConfig.playerSprites.idle;

        if (monsterImage != null && currentConfig.monsterSprites != null && currentConfig.monsterSprites.idle != null)
            monsterImage.sprite = currentConfig.monsterSprites.idle;

        if (playerRect != null)
            playerStartPos = playerRect.anchoredPosition;

        if (monsterRect != null)
            monsterStartPos = monsterRect.anchoredPosition;

        currentValues = new MonsterStatValues(
            currentConfig.hpStartValue,
            currentConfig.damageStartValue,
            currentConfig.speedStartValue,
            currentConfig.intelligenceStartValue
        );

        hpRow.Setup("HP", null, currentConfig.hpStartValue, currentConfig.hpFixed, OnParameterChanged);
        damageRow.Setup("Damage", null, currentConfig.damageStartValue, currentConfig.damageFixed, OnParameterChanged);
        speedRow.Setup("Speed", null, currentConfig.speedStartValue, currentConfig.speedFixed, OnParameterChanged);
        intelligenceRow.Setup("Intelligence", null, currentConfig.intelligenceStartValue, currentConfig.intelligenceFixed, OnParameterChanged);
    }

    private IEnumerator ShowSetupCountdown()
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

    private IEnumerator ShowBattleCountdown()
    {
        if (countdownText == null)
            yield break;

        countdownText.gameObject.SetActive(true);

        countdownText.text = "Battle!";
        yield return new WaitForSeconds(battleCountdownStepDelay);

        countdownText.text = "3";
        yield return new WaitForSeconds(battleCountdownStepDelay);

        countdownText.text = "2";
        yield return new WaitForSeconds(battleCountdownStepDelay);

        countdownText.text = "1";
        yield return new WaitForSeconds(battleCountdownStepDelay);

        countdownText.text = "Fight!";
        yield return new WaitForSeconds(battleCountdownFinalDelay);

        countdownText.gameObject.SetActive(false);
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

        StartCoroutine(PlayBattleSequence(result));
    }

    private IEnumerator PlayBattleSequence(BalanceEvaluationResult result)
    {
        SetInstruction("Watch the battle outcome.");

        yield return StartCoroutine(ShowBattleCountdown());

        switch (result.outcome)
        {
            case BalanceOutcome.TooEasy:
                yield return StartCoroutine(PlayPlayerAttack(killTarget: true));
                yield return StartCoroutine(HandleFailure("Too easy! The monster died too fast. Restarting..."));
                break;

            case BalanceOutcome.TooHard:
                yield return StartCoroutine(PlayPlayerAttack(killTarget: false));
                yield return new WaitForSeconds(betweenActionsDelay);
                yield return StartCoroutine(PlayMonsterAttack(killTarget: true));
                yield return StartCoroutine(HandleFailure("Too hard! The player dies. Restarting..."));
                break;

            case BalanceOutcome.Balanced:
                yield return StartCoroutine(PlayPlayerAttack(killTarget: false));
                yield return new WaitForSeconds(betweenActionsDelay);
                yield return StartCoroutine(PlayMonsterAttack(killTarget: false));
                yield return new WaitForSeconds(betweenActionsDelay);
                yield return StartCoroutine(PlayPlayerAttack(killTarget: true));
                yield return StartCoroutine(HandleSuccess("Balanced! Challenging but fair."));
                break;
        }
    }

    private IEnumerator PlayPlayerAttack(bool killTarget)
    {
        if (currentConfig == null || currentConfig.playerSprites == null || currentConfig.monsterSprites == null)
            yield break;

        if (playerRect == null || monsterRect == null)
            yield break;

        SetSprites(
            currentConfig.playerSprites.idle,
            currentConfig.monsterSprites.idle);

        yield return StartCoroutine(MoveAnchoredPosition(
            playerRect,
            playerStartPos,
            playerStartPos + Vector2.right * moveDistance,
            moveDuration));

        if (currentConfig.playerSprites.attacking != null)
            playerImage.sprite = currentConfig.playerSprites.attacking;

        if (currentConfig.monsterSprites.damaged != null)
            monsterImage.sprite = currentConfig.monsterSprites.damaged;

        yield return new WaitForSeconds(hitPause);

        if (killTarget)
        {
            if (currentConfig.monsterSprites.defeated != null)
                monsterImage.sprite = currentConfig.monsterSprites.defeated;
        }
        else
        {
            if (currentConfig.playerSprites.idle != null)
                playerImage.sprite = currentConfig.playerSprites.idle;

            if (currentConfig.monsterSprites.idle != null)
                monsterImage.sprite = currentConfig.monsterSprites.idle;
        }

        yield return StartCoroutine(MoveAnchoredPosition(
            playerRect,
            playerRect.anchoredPosition,
            playerStartPos,
            returnDuration));
    }

    private IEnumerator PlayMonsterAttack(bool killTarget)
    {
        if (currentConfig == null || currentConfig.playerSprites == null || currentConfig.monsterSprites == null)
            yield break;

        if (playerRect == null || monsterRect == null)
            yield break;

        SetSprites(
            currentConfig.playerSprites.idle,
            currentConfig.monsterSprites.idle);

        yield return StartCoroutine(MoveAnchoredPosition(
            monsterRect,
            monsterStartPos,
            monsterStartPos + Vector2.left * moveDistance,
            moveDuration));

        if (currentConfig.monsterSprites.attacking != null)
            monsterImage.sprite = currentConfig.monsterSprites.attacking;

        if (currentConfig.playerSprites.damaged != null)
            playerImage.sprite = currentConfig.playerSprites.damaged;

        yield return new WaitForSeconds(hitPause);

        if (killTarget)
        {
            if (currentConfig.playerSprites.defeated != null)
                playerImage.sprite = currentConfig.playerSprites.defeated;
        }
        else
        {
            if (currentConfig.playerSprites.idle != null)
                playerImage.sprite = currentConfig.playerSprites.idle;

            if (currentConfig.monsterSprites.idle != null)
                monsterImage.sprite = currentConfig.monsterSprites.idle;
        }

        yield return StartCoroutine(MoveAnchoredPosition(
            monsterRect,
            monsterRect.anchoredPosition,
            monsterStartPos,
            returnDuration));
    }

    private void SetSprites(Sprite playerSprite, Sprite monsterSprite)
    {
        if (playerImage != null && playerSprite != null)
            playerImage.sprite = playerSprite;

        if (monsterImage != null && monsterSprite != null)
            monsterImage.sprite = monsterSprite;
    }

    private IEnumerator MoveAnchoredPosition(RectTransform target, Vector2 from, Vector2 to, float duration)
    {
        if (target == null)
            yield break;

        if (duration <= 0f)
        {
            target.anchoredPosition = to;
            yield break;
        }

        float elapsed = 0f;
        target.anchoredPosition = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        target.anchoredPosition = to;
    }

    private IEnumerator HandleFailure(string message)
    {
        if (!isRunning)
            yield break;

        isRunning = false;
        SetFeedback(message);

        yield return new WaitForSeconds(restartDelay);

        if (rootPanel != null && rootPanel.activeInHierarchy)
            OpenMinigame();
    }

    private IEnumerator HandleSuccess(string message)
    {
        if (!isRunning)
            yield break;

        CompleteMinigame();
        SetFeedback(message);

        yield return new WaitForSeconds(successCloseDelay);

        CloseMinigame();
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
        base.CloseMinigame();
    }
}

/// <summary>
/// Set of battle sprites for one actor.
/// </summary>
[System.Serializable]
public class BattleSpriteSet
{
    public Sprite idle;
    public Sprite attacking;
    public Sprite damaged;
    public Sprite defeated;
}

/// <summary>
/// Inspector configuration for one monster/level.
/// </summary>
[System.Serializable]
public class MonsterLevelConfig
{
    [Header("Presentation")]
    public string levelTitle = "Combat Balance";
    public BattleSpriteSet playerSprites;
    public BattleSpriteSet monsterSprites;

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