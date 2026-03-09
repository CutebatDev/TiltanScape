using System.Collections;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    private Coroutine currentAction;
    private bool cancelAction;

    [SerializeField] private float useDelay;
    public float UseDelay => useDelay;

    public bool IsBusy { get; private set; }

    public void StartAction(IEnumerator action)
    {
        if (IsBusy) return;

        IsBusy = true;
        cancelAction = false;
        currentAction = StartCoroutine(RunAction(action));
    }

    private IEnumerator RunAction(IEnumerator action)
    {
        yield return StartCoroutine(action);
        currentAction = null;
        IsBusy = false;
    }

    public void InterruptAction()
    {
        if (currentAction != null)
        {
            cancelAction = true;
            StopCoroutine(currentAction);
            currentAction = null;
            IsBusy = false;
            Debug.Log("Action Interrupted!");
        }
    }

    public bool ShouldCancelAction() => cancelAction;
}
