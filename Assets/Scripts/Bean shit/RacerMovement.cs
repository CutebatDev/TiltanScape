using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class RacerMovement : MonoBehaviour
{
    private NavMeshAgent navAgent;
    [SerializeField] private Transform target;

    [SerializeField] private UnityEvent onTargetReached;
    private bool didAlreadyReachTarget = false;
    

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(target.position);
    }


    void Update()
    {
        if (didAlreadyReachTarget)
            return;
        
        // Target Reached
        if (navAgent.pathPending == false && navAgent.remainingDistance <= navAgent.stoppingDistance){
            Debug.Log(gameObject.name + " reached");
            onTargetReached?.Invoke();
            didAlreadyReachTarget = true;
        }
    }
}
