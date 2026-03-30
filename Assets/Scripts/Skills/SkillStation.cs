using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActionInteractable))]
public class SkillStation : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private SkillDefinition skill;
    [SerializeField] private int xpReward = 25;
    [SerializeField] private float baseActionTime = 3f;

    [Header("References")]
    [SerializeField] private ActionInteractable interactable;

    void Awake()
    {
        if (!interactable)
            interactable = GetComponent<ActionInteractable>();

        interactable.SetAction(PerformAction);
    }

    private IEnumerator PerformAction()
    {
        yield return new WaitUntil(() =>
            PlayerActionController.Instance != null &&
            PlayerSkills.Instance != null
        );

        while (true)
        {
            if (PlayerActionController.Instance.ShouldCancelAction())
                yield break;

            int level = PlayerSkills.Instance.GetLevel(skill);
            float speedMultiplier = skill.actionSpeed.Evaluate(level);
            float duration = baseActionTime * speedMultiplier;

            Debug.Log($"Speed multiplier: {speedMultiplier}");
            Debug.Log($"Duration: {duration}");

            float timer = 0f;
            while (timer < duration)
            {
                if (PlayerActionController.Instance.ShouldCancelAction())
                {
                    Debug.Log($"{skill.skillName} action interrupted!");
                    yield break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            PlayerSkills.Instance.AddXP(skill, xpReward);
            Debug.Log($"{skill.skillName}: +{xpReward}EXP");
            Debug.Log($"Current {skill.name} level: {PlayerSkills.Instance.GetLevel(skill)}");

            yield return new WaitForSeconds(PlayerActionController.Instance.UseDelay);
        }
    }
}
