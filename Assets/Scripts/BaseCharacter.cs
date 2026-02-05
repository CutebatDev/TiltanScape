using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private RuntimeAnimatorController characterController;
    
    void Start()
    {
        characterAnimator.runtimeAnimatorController = characterController;
    }


}
