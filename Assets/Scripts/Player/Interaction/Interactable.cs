using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    public UnityEvent OnInteract;
    public Transform seat;
    public Transform standSlot;

    [Header("Interaction Animation")]
    [SerializeField] private bool useInteractionAnimation;
    [SerializeField] private EnumAnimations interactionAnimation;

    public void Interact()
    {
        OnInteract?.Invoke();
    }

    public EnumAnimations? GetInteractionAnimation()
    {
        return useInteractionAnimation ? interactionAnimation : null;
    }
}