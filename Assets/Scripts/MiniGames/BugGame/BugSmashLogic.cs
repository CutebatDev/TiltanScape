using UnityEngine;

/// <summary>
/// Pure gameplay logic for the bug smash minigame.
/// This class contains no UI code.
/// It controls level values, bug counts, and win state.
/// </summary>
public class BugSmashLogic
{
    public int TotalBugCount { get; private set; }
    public float MoveSpeed { get; private set; }

    public int SpawnedBugCount { get; private set; }
    public int SmashedBugCount { get; private set; }

    public bool IsCompleted { get; private set; }
    public bool AcceptingInput { get; private set; }

    /// <summary>
    /// Configures the minigame values based on level.
    /// </summary>
    public void ConfigureLevel(int level)
    {
        switch (level)
        {
            case 1:
                TotalBugCount = 7;
                MoveSpeed = 80f;   // Low speed
                break;

            case 2:
                TotalBugCount = 13;
                MoveSpeed = 130f;  // Medium speed
                break;

            case 3:
                TotalBugCount = 17;
                MoveSpeed = 180f;  // High speed
                break;

            default:
                TotalBugCount = 7;
                MoveSpeed = 80f;
                break;
        }
    }

    /// <summary>
    /// Resets round state.
    /// </summary>
    public void ResetRound()
    {
        SpawnedBugCount = 0;
        SmashedBugCount = 0;
        IsCompleted = false;
        AcceptingInput = false;
    }

    /// <summary>
    /// Starts the gameplay input phase.
    /// </summary>
    public void BeginInput()
    {
        AcceptingInput = true;
    }

    /// <summary>
    /// Stops the gameplay input phase.
    /// </summary>
    public void StopInput()
    {
        AcceptingInput = false;
    }

    /// <summary>
    /// Registers one spawned bug.
    /// </summary>
    public void RegisterSpawn()
    {
        SpawnedBugCount++;
    }

    /// <summary>
    /// Registers one smashed bug.
    /// </summary>
    public bool RegisterSmash()
    {
        if (!AcceptingInput)
            return false;

        SmashedBugCount++;

        if (SmashedBugCount >= TotalBugCount)
        {
            IsCompleted = true;
            AcceptingInput = false;
        }

        return true;
    }
}