using UnityEngine;

public class BladeSwing : MonoBehaviour
{
    public float speed = 90f;
    public float maxAngle = 60f;
    public bool invert = false;

    void Update()
    {
        SwingBlade();
    }

    private void SwingBlade()
    {
        float angle = Mathf.Sin(Time.time * speed * Mathf.Deg2Rad) * maxAngle;

        if (invert)
            angle = -angle;

        transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }
}
