using System.Collections;
using UnityEngine;

public class SkillStation : MonoBehaviour
{
    public SkillDefinition skill;
    public int xpReward = 25;
    public float baseActionTime = 3f;

    public void StartPerformAction(PlayerSkills player)
    {
        StartCoroutine(PerformAction(player));
    }

    private IEnumerator PerformAction(PlayerSkills player)
    {
        int level = player.GetLevel(skill);

        float speedMultiplier = skill.actionSpeed.Evaluate(level);
        float duration = baseActionTime * speedMultiplier;

        yield return new WaitForSeconds(duration);

        player.AddXP(skill, xpReward);
    }
}
