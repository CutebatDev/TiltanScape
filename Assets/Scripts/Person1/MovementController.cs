using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    public UnityEvent OnStartedMoving;

    private Controls input;
    private NavMeshAgent agent;
    private Camera mainCamera;

    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask clickableLayers;
    [SerializeField] private float lookRotationSpeed = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;

    private bool isRunning = false;
    private bool wasMoving;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        mainCamera = Camera.main;
        input = new Controls();
    }

    void Update()
    {
        agent.speed = isRunning ? runSpeed : moveSpeed;

        FaceTarget();

        bool isMoving = AgentIsMoving();

        animator.SetFloat("Speed", agent.velocity.magnitude);
        animator.SetBool("IsMoving", isMoving);

        if (isMoving && !wasMoving)
        {
            OnStartedMoving?.Invoke();
        }

        wasMoving = isMoving;
    }

    private void ClickToMove()
    {
        if (!mainCamera)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickableLayers))
        {
            agent.SetDestination(hit.point);

            // Tiny temporary click indicator (debug)
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.position = hit.point + Vector3.up * 0.05f;
            marker.transform.localScale = Vector3.one * 0.2f;
            Destroy(marker.GetComponent<Collider>());
            Destroy(marker, 1f);
        }
    }

    private void OnEnable()
    {
        input.Enable();
        input.Movement.Move.performed += OnMovePerformed;
    }

    private void OnDisable()
    {
        input.Movement.Move.performed -= OnMovePerformed;
        input.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        ClickToMove();
    }

    private void FaceTarget()
    {
        if (!agent.hasPath || agent.pathPending)
            return;

        if (agent.remainingDistance <= agent.stoppingDistance)
            return;

        Vector3 flatDir = agent.destination - transform.position;
        flatDir.y = 0f;

        if (flatDir.sqrMagnitude < 0.0001f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(flatDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    private bool AgentIsMoving()
    {
        return agent.hasPath && agent.velocity.sqrMagnitude > 0.01f;
    }

    private void OnTriggerEnter(Collider other)
    {
        CollisionSystem.HandleTrigger(other.gameObject);
    }

    public float CurrentSpeed => agent.velocity.magnitude;
    public bool IsMoving => AgentIsMoving();
}
