using System.Collections;
using UnityEngine;

public class SkillStation : MonoBehaviour
{
    [SerializeField] PlayerActionController actionController;
    [SerializeField] PlayerSkills skills;

    public SkillDefinition skill;
    public int xpReward = 25;
    public float baseActionTime = 3f;

    public void StartInteract()
    {
        actionController.StartAction(PerformAction());
    }

    private IEnumerator PerformAction()
    {
        Debug.Log($"Current {skill.name} level: {skills.GetLevel(skill)}");

        int level = skills.GetLevel(skill);
        float speedMultiplier = skill.actionSpeed.Evaluate(level);
        float duration = baseActionTime * speedMultiplier;

        Debug.Log($"Speed multiplier: {speedMultiplier}");
        Debug.Log($"Duration: {duration}");

        float timer = 0f;
        while (timer < duration)
        {
            if (actionController.ShouldCancelAction())
            {
                Debug.Log($"{skill.skillName} action interrupted!");
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        skills.AddXP(skill, xpReward);
        Debug.Log($"{skill.skillName}: +{xpReward}EXP");
    }
}
