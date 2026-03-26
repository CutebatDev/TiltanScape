using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Animator animator;
    [SerializeField] private MovementController movementController;

    [Header("Animator Parameters")]
    [SerializeField] private string forwardParam = "ForwardSpeed";      // float
    [SerializeField] private string rotationParam = "RotationSpeed";    // float
    [SerializeField] private string talkingParam = "TalkingEmote";     // trigger
    [SerializeField] private string greetingTrigger = "Greeting";     // trigger
    [SerializeField] private string greetingSpeedParam = "GreetingSpeed"; // float
    [SerializeField] private string upperActiveParam = "UpperActive"; // bool

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        CollisionSystem.OnQuestTrigger += PlayEmote;
        CollisionSystem.OnEnemyTouched += PlayGreet;
    }

    private void OnDisable()
    {
        CollisionSystem.OnQuestTrigger -= PlayEmote;
        CollisionSystem.OnEnemyTouched -= PlayGreet;
    }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void PlayEmote(GameObject other) => PlayEmote();
        public void PlayEmote()
    {
        animator.SetTrigger(talkingParam);
    }

    public void PlayGreet(GameObject other) => PlayGreet();

    public void PlayGreet()
    {
        animator.SetTrigger(greetingTrigger);
    }

    public void SetGreetSpeed(float speed)
    {
        animator.SetFloat(greetingSpeedParam, speed);
    }

    public void SetCharacterSpeed(float forwardSpeed, float rotationSpeed)
    {
        animator.SetFloat(forwardParam, forwardSpeed);
        animator.SetFloat(rotationParam, rotationSpeed);

    }

    private void Update()
    {
        if (!animator) return;
        if (!movementController) return;
        
        float forward = movementController.ForwardSpeed;
        float rotation = movementController.RotationSpeed;


        animator.SetBool(upperActiveParam, movementController.IsTurning);
        animator.SetFloat(forwardParam, forward);
        animator.SetFloat(rotationParam, rotation);

        // Just to prove runtime changes
        animator.SetFloat(greetingSpeedParam, Mathf.PingPong(Time.time, 1f));
    }

    
}
