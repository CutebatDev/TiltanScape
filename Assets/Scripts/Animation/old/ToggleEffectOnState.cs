using UnityEngine;

public class ToggleEffectOnState : StateMachineBehaviour
{
    public string childObjectName = "EnterExitLight";
    private GameObject cached;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (cached == null)
            cached = animator.transform.Find(childObjectName)?.gameObject;

        if (cached != null)
            cached.SetActive(true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (cached == null)
            cached = animator.transform.Find(childObjectName)?.gameObject;

        if (cached != null)
            cached.SetActive(false);
    }
}
