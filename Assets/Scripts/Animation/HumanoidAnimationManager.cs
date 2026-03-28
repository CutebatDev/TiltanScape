using System;
using UnityEngine;

public enum EnumAnimations
{
    Idle,
    Draw,
    Laptop,
    Talk,
    Walk,
    Run
}
public class HumanoidAnimationManager : MonoBehaviour
{
    private static readonly string IsSittingParam = "IsSitting";
    private bool _isSittingBool = false;
    [SerializeField] private Animator animator;

#if UNITY_EDITOR
// this thing allows right-clicking on component and testing stuff quickly

    [ContextMenu("Play Animation/Idle")]
    void AnimIdle() => PlayAnimation(EnumAnimations.Idle);
    
    [ContextMenu("Play Animation/Draw")]
    void AnimDraw() => PlayAnimation(EnumAnimations.Draw);
    
    [ContextMenu("Play Animation/Laptop")]
    void AnimLaptop() => PlayAnimation(EnumAnimations.Laptop);
        
    [ContextMenu("Play Animation/Talk")]
    void AnimTalk() => PlayAnimation(EnumAnimations.Talk);
    
    [ContextMenu("Play Animation/Walk")]
    void AnimWalk() => PlayAnimation(EnumAnimations.Walk);
    
    [ContextMenu("Play Animation/Run")]
    void AnimRun() => PlayAnimation(EnumAnimations.Run);

    [ContextMenu("Play Animation/Toggle Sit")]
    void Sit() => ToggleSitting();
    
#endif
    public void PlayAnimation(EnumAnimations anim)
    {
        animator.SetTrigger(anim.ToString());
    }

    public void ToggleSitting()
    {
        _isSittingBool = !_isSittingBool;
        if(_isSittingBool)
            animator.SetLayerWeight(1, 1);
        else
            animator.SetLayerWeight(1, 0);
    }
}
