using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
public class testscr : MonoBehaviour
{
    private Controls input;
    private NavMeshAgent agent;

    [SerializeField] private LayerMask clickableLayers;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        input = new Controls();
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
        input.Movement.Move.performed += ctx => ClickToMove();
    }
    private void OnDisable()
    {
        input.Disable();
        input.Movement.Move.performed -= ctx => ClickToMove();
    }
}
