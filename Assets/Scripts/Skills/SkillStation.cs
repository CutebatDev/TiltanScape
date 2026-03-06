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
        StartCoroutine(PerformAction());
    }

    private IEnumerator PerformAction()
    {
        Debug.Log("Interaction Started");
        int level = player.GetLevel(skill);

        float speedMultiplier = skill.actionSpeed.Evaluate(level);
        float duration = baseActionTime * speedMultiplier;

        yield return new WaitForSeconds(duration);

        player.AddXP(skill, xpReward);
        Debug.Log($"{xpReward} exp added");
    }
}
