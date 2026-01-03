using UnityEngine;

public class EditorGizmo : MonoBehaviour
{
    public float radius = 1f;
    public Color gizmoColor = Color.green;

    
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
