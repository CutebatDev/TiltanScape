using UnityEngine;
using UnityEngine.InputSystem;

public class AnimatorLayerWeightDriver : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // NOME EXATO da sua layer (como aparece no Animator)
    [SerializeField] private string layerName = "UpperBodyLocomotions";

    [Range(0f, 1f)]
    [SerializeField] private float currentWeight = 0f;

    [SerializeField] private float lerpSpeed = 8f;

    private int layerIndex = -1;
    private float targetWeight = 0f;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        layerIndex = animator.GetLayerIndex(layerName);

        if (layerIndex < 0)
            Debug.LogError($"[AnimatorLayerWeightDriver] Layer '{layerName}' not found. Check spelling!");
    }

    private void Update()
    {
        if (animator == null || layerIndex < 0) return;

        // Toggle com Y: liga/desliga o upper body
        if (Keyboard.current != null && Keyboard.current.yKey.wasPressedThisFrame)
            targetWeight = (targetWeight < 0.5f) ? 1f : 0f;

        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * lerpSpeed);
        animator.SetLayerWeight(layerIndex, currentWeight);
    }
}
