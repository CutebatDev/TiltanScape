using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pure gameplay logic for the memory minigame.
/// This class contains no UI code.
/// It controls sequence generation, validation, and round state.
/// </summary>
public class MemoryGameLogic
{
    public int ColorCount { get; private set; }
    public int ExposureCount { get; private set; }

    public IReadOnlyList<int> GeneratedSequence => generatedSequence;
    public IReadOnlyList<int> PlayerSequence => playerSequence;

    public bool AcceptingInput { get; private set; }
    public bool IsCompleted { get; private set; }

    private readonly List<int> generatedSequence = new();
    private readonly List<int> playerSequence = new();

    /// <summary>
    /// Configures the logic values based on level.
    /// </summary>
    public void ConfigureLevel(int level)
    {
        switch (level)
        {
            case 1:
                ColorCount = 3;
                ExposureCount = 4;
                break;

            case 2:
                ColorCount = 4;
                ExposureCount = 5;
                break;

            case 3:
                ColorCount = 5;
                ExposureCount = 6;
                break;

            default:
                ColorCount = 3;
                ExposureCount = 4;
                break;
        }
    }

    /// <summary>
    /// Resets the round state.
    /// </summary>
    public void ResetRound()
    {
        generatedSequence.Clear();
        playerSequence.Clear();
        AcceptingInput = false;
        IsCompleted = false;
    }

    /// <summary>
    /// Generates a new random sequence for the current round.
    /// </summary>
    public void GenerateSequence()
    {
        generatedSequence.Clear();

        List<int> pool = new();

        for (int i = 0; i < ColorCount; i++)
        {
            pool.Add(i);
        }

        while (pool.Count < ExposureCount)
        {
            pool.Add(Random.Range(0, ColorCount));
        }

        for (int i = 0; i < pool.Count; i++)
        {
            int randomIndex = Random.Range(i, pool.Count);
            int temp = pool[i];
            pool[i] = pool[randomIndex];
            pool[randomIndex] = temp;
        }

        generatedSequence.AddRange(pool);
    }

    /// <summary>
    /// Allows the player to start entering answers.
    /// </summary>
    public void BeginInput()
    {
        playerSequence.Clear();
        AcceptingInput = true;
    }

    /// <summary>
    /// Stops player input.
    /// </summary>
    public void StopInput()
    {
        AcceptingInput = false;
    }

    /// <summary>
    /// Registers one player choice and returns the result of that action.
    /// </summary>
    public MemoryChoiceResult RegisterChoice(int selectedIndex)
    {
        if (!AcceptingInput)
            return MemoryChoiceResult.Ignored;

        playerSequence.Add(selectedIndex);

        int currentStep = playerSequence.Count - 1;

        if (playerSequence[currentStep] != generatedSequence[currentStep])
        {
            AcceptingInput = false;
            return MemoryChoiceResult.Wrong;
        }

        if (playerSequence.Count >= generatedSequence.Count)
        {
            AcceptingInput = false;
            IsCompleted = true;
            return MemoryChoiceResult.Completed;
        }

        return MemoryChoiceResult.CorrectSoFar;
    }
}

/// <summary>
/// Result of one player input action.
/// </summary>
public enum MemoryChoiceResult
{
    Ignored,
    CorrectSoFar,
    Wrong,
    Completed
}