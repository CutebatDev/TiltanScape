using System.Collections;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    public static PlayerActionController Instance { get; private set; }

    private Coroutine currentAction;
    private bool cancelAction;

    [SerializeField] private float useDelay;
    public float UseDelay => useDelay;

    public bool IsBusy { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }    

        Instance = this;
    }

    public void StartAction(IEnumerator action)
    {
        if (IsBusy)
            InterruptAction(); // used to just return;

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
