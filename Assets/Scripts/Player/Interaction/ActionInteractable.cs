using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ActionInteractable : Interactable
{
    [Header("References")]
    [SerializeField] private float baseActionTime = 1f;

    [Header("Optional Events")]
    public UnityEvent OnActionComplete; // Called after interaction completes

    // Delegate to run during interaction
    private Func<IEnumerator> actionCoroutineFunc;

    void OnEnable()
    {
        OnInteract.AddListener(StartInteract);    
    }

    void OnDisable()
    {
        OnInteract.RemoveListener(StartInteract);    
    }

    public void SetAction(Func<IEnumerator> actionFunc)
    {
        actionCoroutineFunc = actionFunc;
    }

    public void StartInteract()
    {
        if (PlayerActionController.Instance == null)
        {
            Debug.LogError("PlayerActionController.Instance is NULL");
            return;
        }

        if (PlayerActionController.Instance.IsBusy || actionCoroutineFunc == null)
            return;

        PlayerActionController.Instance.StartAction(RunAction());
    }

    private IEnumerator RunAction()
    {
        yield return StartCoroutine(actionCoroutineFunc());

        // Trigger completion event if needed
        OnActionComplete?.Invoke();
    }
}
