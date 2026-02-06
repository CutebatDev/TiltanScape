using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
public class testscr : MonoBehaviour
{
    private Controls input;
    private NavMeshAgent agent;

    [SerializeField] private LayerMask clickableLayers;
    [SerializeField] private float lookRotationSpeed = 5f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        input = new Controls();
    }

    void Update()
    {
        FaceTarget();
    }

    private void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            agent.destination = hit.point;

            // tiny temporary click indicator
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
        input.Disable();
        input.Movement.Move.performed -= OnMovePerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        ClickToMove();
    }

    private void FaceTarget()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
            return;

        if (!agent.hasPath)
            return;

        Vector3 flatDir = agent.destination - transform.position;
        flatDir.y = 0f;

        if (flatDir.sqrMagnitude < 0.0001f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(flatDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }
}
