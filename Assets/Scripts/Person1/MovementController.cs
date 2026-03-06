using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    public float interactionRange = 2f;
    private Interactable targetInteractable;

    public UnityEvent OnStartedMoving;

    public float ForwardSpeed => agent.velocity.magnitude;
    public float RotationSpeed => CalculateRotationSpeed();
    public bool IsTurning;

    private Controls input;
    private NavMeshAgent agent;
    private Camera mainCamera;

    [SerializeField] private LayerMask clickableLayers;
    [SerializeField] private float lookRotationSpeed = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;

    private bool isRunning = false;
    private bool wasMoving;
    private Quaternion lastRotation;
    private float rotationSpeed;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        mainCamera = Camera.main;
        input = new Controls();

        agent.stoppingDistance = interactionRange;
    }

    void Update()
    {
        agent.speed = isRunning ? runSpeed : moveSpeed;

        FaceTarget();

        bool isMoving = AgentIsMoving();

        IsTurning = RotationSpeed > 0.1f;

        if (isMoving && !wasMoving)
            OnStartedMoving?.Invoke();

        wasMoving = isMoving;

        CheckInteraction();
    }

    private void ClickToMove()
    {
        if (!mainCamera)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickableLayers))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                targetInteractable = interactable;
                agent.SetDestination(interactable.seat.transform.position);
            }
            else
            {
                targetInteractable = null;
                agent.SetDestination(hit.point);

                DrawClickIndicator(hit);
            }
        }
    }

    private void CheckInteraction()
    {
        if (targetInteractable == null)
            return;

        if (!agent.pathPending && agent.remainingDistance <= interactionRange)
        {
            Debug.Log("dist<=range");

            agent.ResetPath();

            if (targetInteractable.seat != null)
            {
                transform.position = targetInteractable.seat.position;
                transform.rotation = targetInteractable.seat.rotation;
            }

            Debug.Log("before interaction call");
            targetInteractable.Interact();
            targetInteractable = null;
        }
    }

    private void DrawClickIndicator(RaycastHit hit)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = hit.point + Vector3.up * 0.05f;
        marker.transform.localScale = Vector3.one * 0.2f;
        Destroy(marker.GetComponent<Collider>());
        Destroy(marker, 1f);
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

    private float CalculateRotationSpeed()
    {
        // Angle between current rotation and last rotation
        float angle = Quaternion.Angle(transform.rotation, lastRotation);
        lastRotation = transform.rotation;

        // Convert to degrees per second
        return angle / Time.deltaTime;
    }

    public bool IsMoving => AgentIsMoving();
}
