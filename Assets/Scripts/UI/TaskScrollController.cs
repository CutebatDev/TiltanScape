using UnityEngine;

public class TaskScrollController : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private float scrollSpeed = 200f;

    private float minY;
    private float maxY;

    void Start()
    {
        UpdateBounds();
    }

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0)
        {
            Vector2 pos = content.anchoredPosition;
            pos.y += scroll * scrollSpeed * Time.deltaTime;

            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            content.anchoredPosition = pos;
        }
    }

    void UpdateBounds()
    {
        float contentHeight = content.rect.height;
        float viewportHeight = ((RectTransform)transform).rect.height;

        maxY = Mathf.Max(0, contentHeight - viewportHeight);
        minY = 0;
    }
}