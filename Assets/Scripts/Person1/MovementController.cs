using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class MovementController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Camera cam;

    [SerializeField] private InputActionReference clickInput;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;

        if (clickInput == null)
            Debug.Log("clickInput is null");
        else if (clickInput.action == null)
            Debug.Log("clickInput.action is null");
        else
            Debug.Log("clickInput.action exists!");
    }

    private void OnEnable()
    {
        clickInput.action.started += OnClick;
    }

    private void OnDisable()
    {
        clickInput.action.started -= OnClick;
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log("Clicked");

        // Get the mouse position from the InputAction context
        Vector2 mousePos = context.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            agent.SetDestination(hit.point);

            // tiny temporary click indicator
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.position = hit.point + Vector3.up * 0.05f;
            marker.transform.localScale = Vector3.one * 0.2f;
            Destroy(marker.GetComponent<Collider>());
            Destroy(marker, 1f);
        }
    }
}
