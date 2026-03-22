using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ActionInteractable : Interactable
{
    [Header("References")]
    [SerializeField] private PlayerActionController actionController;
    [SerializeField] private float baseActionTime = 1f;

    [Header("Optional Events")]
    public UnityEvent OnActionComplete; // Called after interaction completes

    // Delegate to run during interaction
    private Func<IEnumerator> actionCoroutineFunc;

    void Awake()
    {
        OnInteract.AddListener(StartInteract);    
    }

    public void SetAction(Func<IEnumerator> actionFunc)
    {
        actionCoroutineFunc = actionFunc;
    }

    public void StartInteract()
    {
        if (actionController.IsBusy || actionCoroutineFunc == null)
            return;

        actionController.StartAction(RunAction());
    }

    private IEnumerator RunAction()
    {
        yield return StartCoroutine(actionCoroutineFunc());

        // Trigger completion event if needed
        OnActionComplete?.Invoke();
    }
}
