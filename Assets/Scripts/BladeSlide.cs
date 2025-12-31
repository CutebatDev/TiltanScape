using UnityEngine;

public class BladeSlide : MonoBehaviour
{
    public float speed = 90f;
    public float maxDistance = 10f;
    public bool invert = false;

    void Update()
    {
        SlideBlade();
    }

    private void SlideBlade()
    {
        float position = Mathf.Sin(Time.time * speed) * maxDistance;

        if (invert)
            position = -position;

        transform.localPosition = new Vector3(0, 0, position);
    }
}
