using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class MovementController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Camera cam;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);

                // tiny temporary click indicator
                GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.transform.position = hit.point + Vector3.up * 0.05f;
                marker.transform.localScale = Vector3.one * 0.2f;
                Destroy(marker.GetComponent<Collider>()); // optional, remove collider
                Destroy(marker, 1f); // auto destroy after 1 second
            }
        } 
    }
}
