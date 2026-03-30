using UnityEngine;

/// <summary>
/// Pure gameplay logic for the balancing minigame.
/// This class contains no UI code.
/// It evaluates the chosen values against the monster weights.
/// </summary>
public class BalancingLogic
{
    public const float MinBalancedScore = 50f;
    public const float MaxBalancedScore = 70f;

    /// <summary>
    /// Evaluates the final weighted score.
    /// All slider values should be between 1 and 10.
    /// If all values are 10 and the weights sum to 10, the final score is 100.
    /// </summary>
    public BalanceEvaluationResult Evaluate(MonsterWeights weights, MonsterStatValues chosenValues)
    {
        float totalWeight = weights.TotalWeight;

        if (!Mathf.Approximately(totalWeight, 10f))
        {
            Debug.LogWarning($"MonsterWeights total is {totalWeight}, but it should be 10.");
        }

        float finalScore =
            (chosenValues.hp * weights.hpWeight) +
            (chosenValues.damage * weights.damageWeight) +
            (chosenValues.speed * weights.speedWeight) +
            (chosenValues.intelligence * weights.intelligenceWeight);

        float normalizedDamage = finalScore / 10f; // Converts 0–100 to 0–10 scale

        BalanceOutcome outcome;

        if (finalScore < MinBalancedScore)
        {
            outcome = BalanceOutcome.TooHard;
        }
        else if (finalScore > MaxBalancedScore)
        {
            outcome = BalanceOutcome.TooEasy;
        }
        else
        {
            outcome = BalanceOutcome.Balanced;
        }

        return new BalanceEvaluationResult(finalScore, normalizedDamage, outcome);
    }
}

/// <summary>
/// Runtime values chosen by the player.
/// </summary>
[System.Serializable]
public struct MonsterStatValues
{
    public int hp;
    public int damage;
    public int speed;
    public int intelligence;

    public MonsterStatValues(int hp, int damage, int speed, int intelligence)
    {
        this.hp = Mathf.Clamp(hp, 1, 10);
        this.damage = Mathf.Clamp(damage, 1, 10);
        this.speed = Mathf.Clamp(speed, 1, 10);
        this.intelligence = Mathf.Clamp(intelligence, 1, 10);
    }
}

/// <summary>
/// Internal monster weights. They should sum to 10.
/// </summary>
[System.Serializable]
public struct MonsterWeights
{
    public float hpWeight;
    public float damageWeight;
    public float speedWeight;
    public float intelligenceWeight;

    public MonsterWeights(float hpWeight, float damageWeight, float speedWeight, float intelligenceWeight)
    {
        this.hpWeight = hpWeight;
        this.damageWeight = damageWeight;
        this.speedWeight = speedWeight;
        this.intelligenceWeight = intelligenceWeight;
    }

    public float TotalWeight => hpWeight + damageWeight + speedWeight + intelligenceWeight;
}

/// <summary>
/// Final result of one simulation.
/// </summary>
public readonly struct BalanceEvaluationResult
{
    public readonly float finalScore;
    public readonly float normalizedDamage;
    public readonly BalanceOutcome outcome;

    public BalanceEvaluationResult(float finalScore, float normalizedDamage, BalanceOutcome outcome)
    {
        this.finalScore = finalScore;
        this.normalizedDamage = normalizedDamage;
        this.outcome = outcome;
    }
}

public enum BalanceOutcome
{
    TooHard,
    Balanced,
    TooEasy
}