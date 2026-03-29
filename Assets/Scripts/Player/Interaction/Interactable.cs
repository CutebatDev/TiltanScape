using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnInteract;
    public Transform seat;
    public Transform standSlot;

    public void Interact()
    {
        OnInteract?.Invoke();
    }
}