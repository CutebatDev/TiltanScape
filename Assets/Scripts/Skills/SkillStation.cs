using NUnit.Framework.Internal;
using System.Collections;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(ActionInteractable))]
public class SkillStation : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private MinigameType minigameType;
    [SerializeField] private SkillDefinition skill;

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
            PlayerSkills.Instance != null &&
            MinigameManager.Instance != null
        );

        while (true)
        {
            if (PlayerActionController.Instance.ShouldCancelAction())
                yield break;

            var minigame = MinigameManager.Instance.Get(minigameType);

            if (minigame == null)
            {
                Debug.LogError($"No minigame found for type {minigameType}");
                yield break;
            }
                
            bool finished = false;
            bool success = false;

            void OnCompleted(MinigameBaseUI game)
            {
                finished = true;
                success = true;
            }

            minigame.OnMinigameCompleted += OnCompleted;

            minigame.OpenMinigame();
            
            while (!finished)
            {
                if (PlayerActionController.Instance.ShouldCancelAction())
                {
                    minigame.CloseMinigame();
                    minigame.OnMinigameCompleted -= OnCompleted;
                    yield break;
                }

                yield return null;
            }

            minigame.OnMinigameCompleted -= OnCompleted;

            if (success)
            {
                int xp = minigame.GetExp;
                PlayerSkills.Instance.AddXP(skill, xp);

                Debug.Log($"{skill.skillName}: +{xp} EXP");
            }

            yield return new WaitForSeconds(PlayerActionController.Instance.UseDelay);
        }
    }
}
