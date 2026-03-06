using System.Collections;
using UnityEngine;

public class SkillStation : MonoBehaviour
{
    [SerializeField] private PlayerSkills player;
    public SkillDefinition skill;
    public int xpReward = 25;
    public float baseActionTime = 3f;

    public void Interact()
    {
        player.StartSkillAction(PerformAction());
    }
    //public void StartPerformAction()
    //{
    //    if (player.IsBusy)
    //        return;

    //    StartCoroutine(PerformAction());
    //}

    private IEnumerator PerformAction()
    {
        Debug.Log($"Current {skill.name} level: {player.GetLevel(skill)}");

        int level = player.GetLevel(skill);
        float speedMultiplier = skill.actionSpeed.Evaluate(level);
        float duration = baseActionTime * speedMultiplier;

        Debug.Log($"Speed multiplier: {speedMultiplier}");
        Debug.Log($"Duration: {duration}");

        float timer = 0f;

        while (timer < duration)
        {
            if (player.ShouldCancelAction())
            {
                Debug.Log($"{skill.skillName} action interrupted!");
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        player.AddXP(skill, xpReward);

        Debug.Log($"{skill.skillName}: +{xpReward}EXP");
    }
}
