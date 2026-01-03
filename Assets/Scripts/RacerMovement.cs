using UnityEngine;
using UnityEngine.AI;

public class RacerMovement : MonoBehaviour
{
    private NavMeshAgent navAgent;
    [SerializeField] private Transform target;


    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(target.position);
    }
}
