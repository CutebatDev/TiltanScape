using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGameMinigame : MinigameBaseUI
{
    [Header("Memory Game UI")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Image sequenceDisplay;
    [SerializeField] private GameObject choiceGrid;

    [Header("Choice Button")]
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("Memory Images")]
    [SerializeField] private List<Sprite> availableSprites = new List<Sprite>();

    [Header("Level Settings")]
    [SerializeField] private int level = 1;

    private List<int> generatedSequence = new List<int>();
    private List<int> playerSequence = new List<int>();

    private int colorCount;
    private int exposureCount;
    private const float totalSequenceTime = 6f;
    private bool acceptingInput;

    protected override void Awake()
    {
        base.Awake();
        ConfigureLevel();
    }

    public override void OpenMinigame()
    {
        StopAllCoroutines();
        base.OpenMinigame();
        ConfigureLevel();
        StartCoroutine(RunMemoryGame());
    }

    private void ConfigureLevel()
    {
        switch (level)
        {
            case 1:
                colorCount = 3;
                exposureCount = 4;
                break;

            case 2:
                colorCount = 4;
                exposureCount = 5;
                break;

            case 3:
                colorCount = 5;
                exposureCount = 6;
                break;

            default:
                colorCount = 3;
                exposureCount = 4;
                break;
        }
    }

    private IEnumerator RunMemoryGame()
    {
        ResetVisualState();
        SetInstruction("Memorize the sequence and replicate");

        yield return StartCoroutine(ShowCountdown());

        GenerateSequence();
        yield return StartCoroutine(ShowSequence());

        BuildChoiceGrid();
        ShowChoices();
    }

    private void ResetVisualState()
    {
        generatedSequence.Clear();
        playerSequence.Clear();
        acceptingInput = false;

        countdownText.gameObject.SetActive(false);
        sequenceDisplay.gameObject.SetActive(false);
        choiceGrid.SetActive(false);

        sequenceDisplay.sprite = null;
        SetFeedback("");

        ClearChoiceGrid();
    }

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

    private void GenerateSequence()
    {
        List<int> pool = new List<int>();

        for (int i = 0; i < colorCount; i++)
        {
            pool.Add(i);
        }

        while (pool.Count < exposureCount)
        {
            pool.Add(Random.Range(0, colorCount));
        }

        for (int i = 0; i < pool.Count; i++)
        {
            int randomIndex = Random.Range(i, pool.Count);
            int temp = pool[i];
            pool[i] = pool[randomIndex];
            pool[randomIndex] = temp;
        }

        generatedSequence = pool;
    }

    private IEnumerator ShowSequence()
    {
        sequenceDisplay.gameObject.SetActive(true);

        float singleExposureTime = totalSequenceTime / exposureCount;

        for (int i = 0; i < generatedSequence.Count; i++)
        {
            int spriteIndex = generatedSequence[i];

            if (spriteIndex >= 0 && spriteIndex < availableSprites.Count)
            {
                sequenceDisplay.sprite = availableSprites[spriteIndex];
            }

            yield return new WaitForSeconds(singleExposureTime);
        }

        sequenceDisplay.gameObject.SetActive(false);
    }

    private void BuildChoiceGrid()
    {
        ClearChoiceGrid();

        for (int i = 0; i < colorCount; i++)
        {
            GameObject buttonObject = Instantiate(choiceButtonPrefab, choiceGrid.transform);
            MemoryChoiceButton choiceButton = buttonObject.GetComponent<MemoryChoiceButton>();

            if (choiceButton != null && i < availableSprites.Count)
            {
                choiceButton.Setup(i, availableSprites[i], this);
            }
        }
    }

    private void ClearChoiceGrid()
    {
        for (int i = choiceGrid.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(choiceGrid.transform.GetChild(i).gameObject);
        }
    }

    private void ShowChoices()
    {
        choiceGrid.SetActive(true);
        acceptingInput = true;
        SetFeedback("Repeat the sequence.");
    }

    public void RegisterPlayerChoice(int selectedIndex)
    {
        if (!acceptingInput)
            return;

        playerSequence.Add(selectedIndex);

        int currentStep = playerSequence.Count - 1;

        if (playerSequence[currentStep] != generatedSequence[currentStep])
        {
            StartCoroutine(HandleFailure());
            return;
        }

        if (playerSequence.Count >= generatedSequence.Count)
        {
            StartCoroutine(HandleSuccess());
        }
    }

    private IEnumerator HandleFailure()
    {
        acceptingInput = false;
        SetFeedback("Wrong sequence! Restarting...");

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(RunMemoryGame());
    }

    private IEnumerator HandleSuccess()
    {
        acceptingInput = false;

        string levelText = GetLevelOrdinalText();
        SetFeedback($"You passed your {levelText} art test!");

        yield return new WaitForSeconds(3f);

        CloseMinigame();
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
}