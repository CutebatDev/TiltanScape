using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActionInteractable))]
public class SkillStation : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private PlayerSkills playerSkills;
    [SerializeField] private SkillDefinition skill;
    [SerializeField] private int xpReward = 25;
    [SerializeField] private float baseActionTime = 3f;

    [Header("References")]
    [SerializeField] private PlayerActionController actionController;
    [SerializeField] private ActionInteractable interactable;

    void Awake()
    {
        interactable.SetAction(PerformAction);
    }

    private IEnumerator PerformAction()
    {
        while (true)
        {
            if (actionController.ShouldCancelAction())
                yield break;

            int level = playerSkills.GetLevel(skill);
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

            playerSkills.AddXP(skill, xpReward);
            Debug.Log($"{skill.skillName}: +{xpReward}EXP");
            Debug.Log($"Current {skill.name} level: {playerSkills.GetLevel(skill)}");

            yield return new WaitForSeconds(actionController.UseDelay);
        }
    }
}
