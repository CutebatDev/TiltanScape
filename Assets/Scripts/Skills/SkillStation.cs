using System.Collections;
using UnityEngine;

public class SkillStation : MonoBehaviour
{
    [SerializeField] private PlayerSkills player;
    public SkillDefinition skill;
    public int xpReward = 25;
    public float baseActionTime = 3f;

    public void StartPerformAction()
    {
        if (player.IsBusy)
            return;

        StartCoroutine(PerformAction());
    }

    private IEnumerator PerformAction()
    {
        player.SetIsBusy(true);

        Debug.Log($"Current {skill.name} level: {player.GetLevel(skill)}");
        int level = player.GetLevel(skill);

        float speedMultiplier = skill.actionSpeed.Evaluate(level);
        float duration = baseActionTime * speedMultiplier;

        Debug.Log($"Speed multiplier: {speedMultiplier}");
        Debug.Log($"Duration: {duration}");

        yield return new WaitForSeconds(duration);

        player.AddXP(skill, xpReward);

        Debug.Log($"{xpReward} exp added");

        player.SetIsBusy(false);
    }
}
