using UnityEngine;

public class SkillXP
{
    public const int maxLevel = 99;
    public const float exponentBase = 2f;
    public const float exponentDivisor = 7f;
    public const float scalingFactor = 300f;
    public const float finalDivisor = 4f;

    public static int GetXPForLevel(int level)
    {
        int points = 0;

        for (int i = 1; i < level; i++)
        {
            points += Mathf.FloorToInt(i + scalingFactor * Mathf.Pow(exponentBase, i / exponentDivisor));
        }

        return Mathf.FloorToInt(points / finalDivisor);
    }

    public static int GetLevelForXP(int xp)
    {
        int level = 1;

        while (level < maxLevel && xp >= GetXPForLevel(level + 1))
            level++;

        return level;
    }
}
