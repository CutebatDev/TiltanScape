using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Animator animator;

    [Header("Animator Parameters")]
    [SerializeField] private string forwardParam = "ForwardSpeed";      // float
    [SerializeField] private string rotationParam = "RotationSpeed";    // float
    [SerializeField] private string talkingParam = "TalkingEmote";     // bool
    [SerializeField] private string greetingTrigger = "Greeting";     // trigger
    [SerializeField] private string greetingSpeedParam = "GreetingSpeed"; // float

    [Header("Tuning")]
    [SerializeField] private float forwardValue = 1f;
    [SerializeField] private float rotationValue = 1f;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

        public void PlayEmote()
    {

        animator.SetBool("_talkingHash", true);
        Invoke(nameof(StopEmoteInternal), 0.1f);
    }

    public void PlayGreet()
    {
        animator.SetTrigger("_greetingHash");
    }

    public void SetGreetSpeed(float speed)
    {
        animator.SetFloat("_greetingSpeedHash", speed);
    }

    public void SetCharacterSpeed(float forwardSpeed, float rotationSpeed)
    {
        animator.SetFloat("_forwardHash", forwardSpeed);
        animator.SetFloat("_rotationHash", rotationSpeed);

    }



    private void StopEmoteInternal()
    {
        animator.SetBool("_talkingHash", false);
    }


    private void Update()
    {
        if (animator == null) return;

        // the movement input (WASD)
        float forward = 0f;
        float rotation = 0f;

        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed) forward += forwardValue;
            if (kb.sKey.isPressed) forward -= forwardValue;

            if (kb.dKey.isPressed) rotation += rotationValue;
            if (kb.aKey.isPressed) rotation -= rotationValue;

            // Toggle TalkingEmot (T)
            if (kb.tKey.wasPressedThisFrame)
            {
                bool current = animator.GetBool(talkingParam);
                animator.SetBool(talkingParam, !current);
            }

            // Trigger Greeting (G)
            if (kb.gKey.wasPressedThisFrame)
            {
                animator.SetTrigger(greetingTrigger);
            }
        }

        animator.SetFloat(forwardParam, forward);
        animator.SetFloat(rotationParam, rotation);

        // Just to prove runtime changes
        animator.SetFloat(greetingSpeedParam, Mathf.PingPong(Time.time, 1f));
    }

    
}
